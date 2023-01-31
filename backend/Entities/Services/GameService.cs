using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
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
    IHubContext<GameHub> _gameHub;
    
    public GameService(KviziramContext context, Utility utility, IQuizService quiz, IMatchService match, IAccountService account, IHubContext<GameHub> gameHub) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _quiz = quiz;
        _match = match;
        _account = account;
        _gameHub = gameHub;
    }

    public async Task<Match?> CreateGameAsync(Match game) {
        if (game != null && game.QuizID != null) {   
            //Game setup (user je uneo IsSearchable i QuizID)  
            game.ID = Guid.NewGuid();
            game.InviteCode = _util.CreateInviteCode();
            game.GameState = GameState.Waiting;
            game.Created = DateTime.Now;
            if(_util.CallerAccountExists() != null) game.HostID = _util.CallerAccountExists().ID;


            //PROVERI DAL SU PITANJA U REDIS
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
                        AccountPoco acc = await _account.GetAccountAsync(Guid.Parse(playerID));

                        if (await _redis.KeyExistsAsync(_util.RK_Scores(inviteCode))) {
                            if (await _redis.HashExistsAsync(_util.RK_Lobby(inviteCode), playerID)) {
                                await _redis.HashSetAsync(_util.RK_Lobby(inviteCode), playerID, playerSID);

                                await _gameHub.Clients.Group(inviteCode).SendAsync("receiveReconnected", playerID);
                                await _redis.PublishAsync(_util.RK_GameWatcher(inviteCode), $"Chat:{acc.Username} came back.");
                            }
                            else {
                                Console.WriteLine("Ovaj korisnik nije bio u lobby kad je igra zapoceta");
                                await _gameHub.Clients.Group(inviteCode).SendAsync("receiveKickout", inviteCode);
                                await _redis.PublishAsync(_util.RK_GameWatcher(inviteCode), $"Chat:{acc.Username} tried to join the game.");
                            }
                        } else {
                            await _redis.HashSetAsync(_util.RK_Lobby(inviteCode), playerID, playerSID);

                            await _gameHub.Clients.Group(inviteCode).SendAsync("receiveConnected", playerID);
                            await _redis.PublishAsync(_util.RK_GameWatcher(inviteCode), $"Chat:{acc.Username} joined the game.");

                        }
                        break;
                    }

                    case "Disconnected": { 
                        string[] SIDandID = msgOperation[1].Split('|');
                        string playerSID = SIDandID[0];
                        string playerID = SIDandID[1];
                        await _redis.HashDeleteAsync(_util.RK_Lobby(inviteCode), playerID);
                        await _redis.HashDeleteAsync(_util.RK_PlayersAnswered(inviteCode), playerID);   

                        await _gameHub.Clients.Group(inviteCode).SendAsync("receiveDisconnected", playerID);

                        if (await _redis.HashLengthAsync(_util.RK_Lobby(inviteCode)) == 0 && game != null) {
                            if (await _redis.KeyExistsAsync(_util.RK_Scores(inviteCode))) {
                                game.GameState = GameState.Unfinished;
                                await SaveGameToHistoryAsync(game);
                            }
                            await RemoveGameFromRedisAsync(inviteCode); 
                            await masterOfDisaster.UnsubscribeAsync(channel);                           
                        }
                        break;
                    }

                    case "Answered": {
                        string[] IDAndAnswer = msgOperation[1].Split('|');
                        string playerID = IDAndAnswer[0];
                        int playerAnswer = Int32.Parse(IDAndAnswer[1]);

                        await _redis.HashSetAsync(_util.RK_PlayersAnswered(inviteCode), playerID, playerAnswer);

                        long LobbyLength = await _redis.HashLengthAsync(_util.RK_Lobby(inviteCode));
                        long AnsweredLength = await _redis.HashLengthAsync(_util.RK_PlayersAnswered(inviteCode));

                        //Svi odgovorili, ako neko ne odgovara, force-uj ga da posalje -1 kao odgovor
                        if (LobbyLength == AnsweredLength && game != null) {
                            int index = (int) (await _redis.StringGetAsync(_util.RK_CurrentQuestion(inviteCode)));
                            //Potrebno da vidimo koliko poena pitanje iznosi
                            string res = (await _redis.ListGetByIndexAsync(_util.RK_Questions(game.QuizID), index)).ToString();
                            QuestionDto? question = _util.DeserializeQuestion(res);
                            if (question != null) {
                                //Tacan odgovor
                                int correctAnswer = Int32.Parse((await _redis.ListGetByIndexAsync(_util.RK_QuestionsAnswers(game.QuizID), index)).ToString());

                                var finalAnswers = await _redis.HashGetAllAsync(_util.RK_PlayersAnswered(inviteCode));
                                foreach(var elem in finalAnswers) {
                                    //Dal je odgovor tacan
                                    if (Int32.Parse(elem.Value.ToString()) == correctAnswer) {
                                        await _redis.SortedSetIncrementAsync(_util.RK_Scores(inviteCode), elem.Name, (double) question.Points);
                                    }
                                }
                                await _redis.KeyDeleteAsync(_util.RK_PlayersAnswered(inviteCode));
                                await _redis.StringIncrementAsync(_util.RK_CurrentQuestion(inviteCode));

                                await _gameHub.Clients.Group(inviteCode).SendAsync("receiveAnswered", playerID);
                            }   

                            long numOfQuestions = await _redis.ListLengthAsync(_util.RK_Questions(game.QuizID));
                            if ((index+1) == numOfQuestions) {
                                game.GameState = GameState.Finished;
                                Console.WriteLine("Server: Iznad SaveGameToHistory");
                                await SaveGameToHistoryAsync(game);
                                Console.WriteLine("Server: Ispod SaveGameToHistory");
                                await RemoveGameFromRedisAsync(inviteCode);

                                await _gameHub.Clients.Group(inviteCode).SendAsync("receiveFinishedGame", game.ID);
    
                                await masterOfDisaster.UnsubscribeAsync(channel);                          
                            } else {
                                await _gameHub.Clients.Group(inviteCode).SendAsync("receiveNextQuestion", game.QuizID);
                            }                                      
                        }
                        break;
                    }

                    case "Chat": {
                        string chatMessage = msgOperation[1];
                        await _redis.ListLeftPushAsync(_util.RK_Chat(inviteCode), chatMessage);
                        await _gameHub.Clients.Group(inviteCode).SendAsync("receiveChatMessage", chatMessage);
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
                    await _redis.StringSetAsync(_util.RK_CurrentQuestion(inviteCode), 0, Duration.Questions);

                    match.GameState = GameState.Playing;
                    await _redis.StringSetAsync(_util.RK_Game(inviteCode), _util.SerializeMatch(match), Duration.Game);

                    double PublicGameKey = match.Created.Value.ToOADate();
                    await _redis.SortedSetRemoveRangeByScoreAsync(_util.RK_PublicMatches, PublicGameKey, PublicGameKey);

                    await _gameHub.Clients.Group(inviteCode).SendAsync("receiveGameStarted", inviteCode);
                    return Msg.GameStarted;
                }
                return Msg.NoLobby;
            }
            return Msg.NoStartGame;
        }
        return Msg.NoGame;
    }

    public async Task<Match?> GetGameAsync(string inviteCode) {
        Match? game = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(inviteCode)));
        return game;
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
            Console.WriteLine("Server: Usao u SaveGameToHistory i prosao prvi IF");
            var playerIDsAndScores = await _redis.SortedSetRangeByScoreWithScoresAsync(_util.RK_Scores(game.InviteCode), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            game.SetPlayerIDsScores = new Dictionary<Guid, int>();
            game.Guests = new Dictionary<string, int>();

            Quiz? tempQuiz =  await _quiz.GetQuizAsync((Guid) game.QuizID);
            int maxPoints = 0;
            if (tempQuiz != null && tempQuiz.Questions != null) {
                maxPoints = tempQuiz.Questions.Select(q => q.Points).Sum();
                Console.WriteLine($"Server: maxPoints = ${maxPoints}");
            }

            foreach(SortedSetEntry playerScore in playerIDsAndScores) {
                Console.WriteLine($"Server: PlayerID: {playerScore.Element} -> PlayerScore: {playerScore.Score}");

                Guid tempGUID = Guid.Parse(playerScore.Element.ToString());
                Console.WriteLine($"Server: tempGuid = {tempGUID}");

                if (await _account.AccountExistsAsync(tempGUID)) {
                    Console.WriteLine("Server: Account sa tempGuid postoji i usli smo u IF");
                    game.SetPlayerIDsScores.Add(tempGUID, (int) playerScore.Score);
                    Console.WriteLine("Server: Iznad dodavanja Achievement-a ako treba");
                    if (playerScore.Score == maxPoints && tempQuiz != null && tempQuiz.Achievement != null) {
                        Console.WriteLine("Server: U IF delu za dodavanje Achievement-a");
                        await _account.SetUpdateAchievementAsync(tempGUID, tempQuiz.Achievement.ID);
                    }
                    Console.WriteLine("Server: Ispod IF dela za dodavanje Achievement-a");
                } else {
                    game.Guests.Add(tempGUID.ToString(), (int) playerScore.Score);
                    Console.WriteLine("Server: Guest je dodat, tempGuid nije bio account u Neo4j");
                }
            }

            if (game.GameState == GameState.Unfinished) {
                game.WinnerID = Guid.Empty;
                Console.WriteLine($"Server: Game je Unfinished i postavljamo praznog Winner-a {game.WinnerID}");
            }
            else {
                game.WinnerID = Guid.Parse(playerIDsAndScores.ElementAt(0).Element.ToString());
                Console.WriteLine($"Server: Game je Finished i Winner je {game.WinnerID}");
            }

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

    public async Task<List<AccountPoco>> GetGameLobbyAsync(string inviteCode) {
        var lobby = await _redis.HashGetAllAsync(_util.RK_Lobby(inviteCode));
        List<AccountPoco> accList = new List<AccountPoco>();
        foreach(var elem in lobby) {
            var redisAcc = await _redis.StringGetAsync(elem.Value.ToString());
            AccountPoco? acc = _util.DeserializeAccountPoco(redisAcc);
            if (acc != null) accList.Add(acc);
        }
        return accList;
    }

    public async Task<List<GameScore>> GetGameScoresAsync(string inviteCode) {
        var scores = await _redis.SortedSetRangeByScoreWithScoresAsync(_util.RK_Scores(inviteCode), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
        List<GameScore> scoresList = new List<GameScore>();
        foreach(var elem in scores) {
            AccountPoco? acc = await _account.GetAccountAsync(Guid.Parse(elem.Element.ToString()));
            GameScore score = new GameScore();

            score.Account = acc;
            score.Score = (int) elem.Score;
            scoresList.Add(score);
        }
        return scoresList;
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

    public async Task<List<string>> GetPlayersAnswered(string inviteCode) {
        var playersAnswered = await _redis.HashKeysAsync(_util.RK_PlayersAnswered(inviteCode));
        return playersAnswered.Select(playerID => playerID.ToString()).ToList();
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

    public async Task SendInviteAsync(Guid auID, string inviteCode) {
        GameInviteDto invite = new GameInviteDto();
        invite.FromUser = _util.CallerAccountExists();
        invite.Game = await GetGameInformationAsync(inviteCode);
        string inviteSerialized = JsonSerializer.Serialize<GameInviteDto>(invite);

        if (invite.Game != null)
            await _redis.SortedSetAddAsync(_util.RK_Invite(auID.ToString()), inviteSerialized, invite.Game.Created.ToOADate());
        await _util.SendGameInvite(auID, inviteSerialized);
    }

    public async Task<List<GameInviteDto>?> GetAllInvitesAsync(Guid auID) {
        var inviteList = await _redis.SortedSetRangeByScoreWithScoresAsync(_util.RK_Invite(auID.ToString()), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
        List<GameInviteDto> invites = new List<GameInviteDto>();

        foreach(var elem in inviteList) {
            GameInviteDto? invite = JsonSerializer.Deserialize<GameInviteDto>(elem.Element.ToString());
            if (invite != null && invite.Game != null) {
                Match? game = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RK_Game(invite.Game.InviteCode)));
                if (game == null || game.GameState == GameState.Playing)
                    await _redis.SortedSetRemoveRangeByScoreAsync(_util.RK_Invite(auID.ToString()), invite.Game.Created.ToOADate(), invite.Game.Created.ToOADate());
                else
                    invites.Add(invite);
            }
        }
        return invites;
    }

    public async Task ClickInviteAsync(GameInviteDto invite) {
        if (_context.AccountCaller != null && invite.Game != null) {
            await _redis.SortedSetRemoveRangeByScoreAsync(_util.RK_Invite(_context.AccountCaller.ID.ToString()), invite.Game.Created.ToOADate(), invite.Game.Created.ToOADate());
        }
    }

    public async Task CreatePubSubAsync(string channelName) {
        Console.WriteLine("Zapocinjem kreiranje masterOfDisaster");
        ISubscriber masterOfDisaster = _context.Redis.GetSubscriber();
        await masterOfDisaster.SubscribeAsync(channelName, async (channel, message) => {
            Console.WriteLine(await Task.FromResult($"Primio poruku: {channel} | {message}"));
        });
        Console.WriteLine("Zavrsio sa masterOfDisaster");
    }

    public async Task TestPubSubAsync(string channelName, string msg) {
        Console.WriteLine($"Spremam se da posaljem {channelName} poruku {msg}");
        await _redis.PublishAsync(channelName, msg);
        Console.WriteLine("Poruka poslata");
    }
}
