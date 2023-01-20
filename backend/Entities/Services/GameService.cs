using System.Text.Json;
using Neo4jClient;
using StackExchange.Redis;

public class GameService: IGameService
{
    IDatabase _redis;
    IGraphClient _neo;
    Utility _util;
    IQuizService _quiz;
    IMatchService _match;
    IAccountService _account;
    KviziramContext _context;
    
    public GameService(KviziramContext context, Utility utility, IQuizService quiz, IMatchService match, IAccountService account) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _quiz = quiz;
        _match = match;
        _account = account;
    }

    public async Task<Match?> CreateGameAsync(Match game) {
        if (game != null && game.QuizID != null) {   
            //Game setup (user je uneo IsSearchable i QuizID)  
            game.ID = Guid.NewGuid();
            game.InviteCode = _util.CreateInviteCode();
            game.GameState = GameState.Waiting;
            game.Created = DateTime.Now;
            if(_util.CallerAccountExists() != null) game.HostID = _util.CallerAccountExists().ID;

            string RK_Questions = _util.RK_Questions(game.QuizID);
            Quiz? tempQuiz =  await _quiz.GetQuizAsync((Guid) game.QuizID);
            if (tempQuiz != null) {
                //Ako ne postoji questions vec ucitani u redis onda gi ucitaj za brzi rad da ne povlacimo iz neo svaki put
                if(!(await _redis.KeyExistsAsync(RK_Questions))) {
                    if (tempQuiz.Questions != null) {
                        foreach(QuestionDto question in tempQuiz.Questions) {
                            await _redis.ListRightPushAsync(_util.RK_QuestionsAnswers(game.QuizID), question.Answer);
                            //Nema vreme da se ovo deli u klase, it is what it is
                            question.Answer = -1;
                            await _redis.ListRightPushAsync(RK_Questions, _util.SerializeQuestion(question));                            
                        }
                        await _redis.KeyExpireAsync(_util.RK_QuestionsAnswers(game.QuizID), Duration.QuestionsAnswers);
                        await _redis.KeyExpireAsync(RK_Questions, Duration.Questions);

                    } else throw new KviziramException(Msg.NoQuestions);
                }

                if (game.IsSearchable) {
                    GameDto? gameDto = await ConvertMatchToGameDtoAsync(game);
                    if (gameDto != null) {
                        string gameDTOjson = _util.SerializeGameDto(gameDto);
                        await _redis.SortedSetAddAsync(_util.RK_PublicMatches, gameDTOjson, game.Created.Value.ToOADate());
                    }
                }
                
                await _redis.ListLeftPushAsync(_util.RK_Chat(game.InviteCode), Msg.ChatWelcome);
                await _redis.KeyExpireAsync(_util.RK_Chat(game.InviteCode), Duration.Chat);

                await _redis.StringSetAsync(_util.RK_Game(game.InviteCode), _util.SerializeMatch(game), Duration.Game);
            } else throw new KviziramException(Msg.NoQuiz);

            #region PubSub
            //############################################################################
            ISubscriber masterOfDisaster = _context.Redis.GetSubscriber();
            await masterOfDisaster.SubscribeAsync(_util.RK_GameWatcher(game.InviteCode), async (channel, message) => {
                string[] msgOperation = message.ToString().Split(":", 2);;
                Console.Write(msgOperation[0] + " ----- " + msgOperation[1]);

                string inviteCode = channel.ToString().Split(':')[1];
                Console.WriteLine(inviteCode);

                Match? game = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode)));

                //Bogy pomogy ako ovo radi
                switch(msgOperation[0]) {
                    case "Connected": {
                        string[] SIDandID = msgOperation[1].Split('|');
                        string playerSID = SIDandID[0];
                        string playerID = SIDandID[1];
                        Console.WriteLine(playerSID + " ----- " + playerID);

                        if (await _redis.KeyExistsAsync(_util.RK_Scores(inviteCode))) {
                            if (await _redis.HashExistsAsync(_util.RK_Lobby(inviteCode), playerID))
                                await _redis.HashSetAsync(_util.RK_Lobby(inviteCode), playerID, playerSID);
                            else 
                                Console.WriteLine("Ovaj korisnik nije bio u lobby kad je igra zapoceta");
                        } else {
                            await _redis.HashSetAsync(_util.RK_Lobby(inviteCode), playerID, playerSID);
                        }

                        break;
                    }

                    case "Disconnected": { 
                        string[] SIDandID = msgOperation[1].Split('|');
                        string playerSID = SIDandID[0];
                        string playerID = SIDandID[1];
                        await _redis.HashDeleteAsync(_util.RK_Lobby(inviteCode), playerID);   

                        if (await _redis.HashLengthAsync(_util.RK_Lobby(inviteCode)) == 0 && game != null) {
                            if (await _redis.KeyExistsAsync(_util.RK_Scores(inviteCode))) {
                                game.GameState = GameState.Unfinished;
                                await SaveGameToHistoryAsync(game);
                            }
                            await RemoveGameFromRedisAsync(inviteCode);                            
                        }
                        break;
                    }

                    case "Answered": {
                        break;
                    }

                    case "Chat": {
                        break;
                    }

                    default: {
                        Console.WriteLine("How did we end up here?");
                        break;
                    }

                }

            });
            //############################################################################
            #endregion

            return _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(game.InviteCode)));
        }
        throw new KviziramException(Msg.NoMatch);
    }

    public async Task<List<GameDto>?> GetPublicGamesAsync(FromToDate fromToDate, int skip, int limit, bool asc) {
        double fromDate = fromToDate.FromDate.ToOADate();
        double toDate = fromToDate.ToDate.ToOADate();
        Order order = (asc) ? Order.Ascending : Order.Descending;

        var gamesObj = await _redis.SortedSetRangeByScoreAsync(_util.RK_PublicMatches, fromDate, toDate, Exclude.None, order, skip, limit);
        if (gamesObj == null)
            return null;

        List<GameDto> gameList = new List<GameDto>();
        //Nije vreme UTC, pazi kako setup-ujes (+01:00h)
        foreach(RedisValue value in gamesObj) {
            GameDto? gameDto = _util.DeserializeGameDto(value);
            if (gameDto != null) gameList.Add(gameDto);
        }
        return gameList;
    }

    public async Task<string> StartGameAsync(string inviteCode) {
        Match? match = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode)));

        if (match != null && match.HostID != null && match.Created != null) {
            if (match.HostID == _util.CallerAccountExists().ID) {
                var playersList = await _redis.HashKeysAsync(_util.RK_Lobby(inviteCode));

                if (playersList != null) {
                    foreach(RedisValue playerGuid in playersList) {
                        await _redis.SortedSetAddAsync(_util.RK_Scores(inviteCode), playerGuid, 0.00);
                        await _redis.ListLeftPushAsync(_util.RK_PlayedGames(playerGuid), inviteCode);
                    }

                    await _redis.KeyExpireAsync(_util.RK_Lobby(inviteCode), Duration.GameScore);
                    await _redis.StringSetAsync(_util.RK_PlayersAnswered(inviteCode), 0, Duration.NumberOfAnswers);
                    await _redis.StringSetAsync(_util.RK_CurrentQuestion(inviteCode), 0, Duration.Questions);

                    match.GameState = GameState.Playing;
                    await _redis.StringSetAsync(_util.RK_Game(inviteCode), _util.SerializeMatch(match), Duration.Game);

                    double PublicGameKey = match.Created.Value.ToOADate();
                    await _redis.SortedSetRemoveRangeByScoreAsync(_util.RK_PublicMatches, PublicGameKey, PublicGameKey);

                    return Msg.GameStarted;
                }
                return Msg.NoLobby;
            }
            return Msg.NoStartGame;
        }
        return Msg.NoGame;
    }

    public async Task<GameDto?> GetGameInformationAsync(string inviteCode) {
        //Ako ovo vrati null, zovi getmatch u neo4j
        Match? game = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode)));
        return await ConvertMatchToGameDtoAsync(game);
    }

    public async Task<GameDto?> ConvertMatchToGameDtoAsync(Match? game) {
        if (game != null && game.QuizID != null && game.Created != null && game.InviteCode != null) {
            Quiz? tempQuiz =  await _quiz.GetQuizAsync((Guid) game.QuizID);
            if (tempQuiz != null) {
                GameDto gameDTO = new GameDto();
                gameDTO.Created = game.Created.Value;
                gameDTO.InviteCode = game.InviteCode;
                gameDTO.HostName = _util.CallerAccountExists().Username;
                gameDTO.QuizName = (tempQuiz.Name != null) ? tempQuiz.Name : string.Empty;
                gameDTO.CategoryName = (tempQuiz.Category != null && tempQuiz.Category.Name != null) ? tempQuiz.Category.Name : string.Empty;
                gameDTO.TrophyName = (tempQuiz.Achievement != null && tempQuiz.Achievement.Name != null) ? tempQuiz.Achievement.Name : string.Empty;

                return gameDTO;
            }
        }
        return null;       
    }

    public async Task<Match?> SaveGameToHistoryAsync(Match game) {
        if (game.InviteCode != null && game.QuizID != null) {
            var playerIDsAndScores = await _redis.SortedSetRangeByScoreWithScoresAsync(_util.RK_Scores(game.InviteCode), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            game.SetPlayerIDsScores = new Dictionary<Guid, int>();
            game.Guests = new Dictionary<string, int>();

            int maxPoints = (await _redis.ListRangeAsync(_util.RK_QuestionsAnswers(game.QuizID))).Select(x => (int) x).Sum();
            Quiz? tempQuiz =  await _quiz.GetQuizAsync((Guid) game.QuizID);

            foreach(SortedSetEntry playerScore in playerIDsAndScores) {
                Guid tempGUID = Guid.Parse(playerScore.Element.ToString());
                if (await _account.AccountExistsAsync(tempGUID)) {
                    game.SetPlayerIDsScores.Add(tempGUID, (int) playerScore.Score);
                    if (playerScore.Score == maxPoints && tempQuiz != null && tempQuiz.Achievement != null)
                        await _account.SetUpdateAchievementAsync(tempGUID, tempQuiz.Achievement.ID);
                } else {
                    game.Guests.Add(tempGUID.ToString(), (int) playerScore.Score);
                }
            }

            if (game.GameState == GameState.Unfinished) 
                game.WinnerID = Guid.Empty;
            else 
                game.WinnerID = Guid.Parse(playerIDsAndScores.ElementAt(0).ToString());

            await _match.SaveMatchAsync(game);
            return await _match.GetMatchAsync(game.ID);
        }
        
        return null;
    }

    public async Task<bool> RemoveGameFromRedisAsync(string inviteCode) {
        Match? match = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode)));
        if (match != null && match.Created != null) {
            double PublicGameKey = match.Created.Value.ToOADate();
            await _redis.SortedSetRemoveRangeByScoreAsync(_util.RK_PublicMatches, PublicGameKey, PublicGameKey);
            //Dis gonna be ugly
            RedisKey[] keys = new RedisKey[] {
                _util.RK_Game(inviteCode),
                _util.RK_Lobby(inviteCode),
                _util.RK_Scores(inviteCode),
                _util.RK_Chat(inviteCode),
                _util.RK_CurrentQuestion(inviteCode),
                _util.RK_PlayersAnswered(inviteCode)
            };
            await _redis.KeyDeleteAsync(keys);

            return true;
        } 
        return false;
    }

    public async Task<Dictionary<string, string>> GetGameLobbyAsync(string inviteCode) {
        var lobby = await _redis.HashGetAllAsync(_util.RK_Lobby(inviteCode));
        Dictionary<string, string> dic = new Dictionary<string, string>();
        foreach(var elem in lobby)
            dic.Add(elem.Name.ToString(), elem.Value.ToString());
        return dic;
    }

    public async Task<Dictionary<string, int>> GetGameScoresAsync(string inviteCode) {
        var scores = await _redis.SortedSetRangeByScoreWithScoresAsync(_util.RK_Scores(inviteCode), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
        Dictionary<string, int> dic = new Dictionary<string, int>();
        foreach(var elem in scores)
            dic.Add(elem.Element.ToString(), (int) elem.Score);
        return dic;
    }

    public async Task<List<string>> GetGameChatAsync(string inviteCode, int start = 0, int stop = 100) {
        var chat = await _redis.ListRangeAsync(_util.RK_Chat(inviteCode), start, stop);
        return chat.Select(x => x.ToString()).ToList();
    }

    public async Task<QuestionDto?> GetGameCurrentQuestionAsync(string inviteCode, Guid quizID) {
        int index = (int) (await _redis.StringGetAsync(_util.RK_CurrentQuestion(inviteCode)));
        string res = (await _redis.ListGetByIndexAsync(_util.RK_Questions(quizID), index)).ToString();
        return _util.DeserializeQuestion(res);
    }

    public async Task<List<GameDto>?> GetLastPlayedGamesAsync(Guid playerGuid) {
        var res = await _redis.ListRangeAsync(_util.RK_PlayedGames(playerGuid.ToString()));

        if (res != null) {                
            List<GameDto> gamesPlayed = new List<GameDto>();
            foreach(var inviteCode in res) {
                Match? match = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode.ToString())));
                GameDto? gameDto = await ConvertMatchToGameDtoAsync(match);
                if (gameDto != null) gamesPlayed.Add(gameDto);
            }
            return gamesPlayed;
        }
        return null;            
    }

    public async Task AddToLobby(string inviteCode, Guid auID, string sid) {
        await _redis.HashSetAsync(_util.RK_Lobby(inviteCode), auID.ToString(), sid);
    }
}
