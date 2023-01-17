using System.Text.Json;
using Neo4jClient;
using StackExchange.Redis;

public class GameService: IGameService
{
    IDatabase _redis;
    IGraphClient _neo;
    Utility _util;
    IQuizService _quiz;
    
    public GameService(KviziramContext context, Utility utility, IQuizService quiz) {
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _quiz = quiz;
    }

    public async Task<Match?> CreateGameAsync(Match game) {
        if (game != null && game.QuizID != null) {   
            //Game setup (user je uneo IsSearchable i QuizID)  
            game.ID = Guid.NewGuid();
            game.InviteCode = _util.CreateInviteCode();
            game.GameState = GameState.Waiting;
            game.Created = DateTime.Now;
            if(_util.CallerAccountExists() != null) game.HostID = _util.CallerAccountExists().ID;

            string RedisKeyQuestions = _util.RedisKeyQuestions(game.QuizID);
            Quiz? tempQuiz =  await _quiz.GetQuizAsync((Guid) game.QuizID);
            if (tempQuiz != null) {
                //Ako ne postoji questions vec ucitani u redis onda gi ucitaj za brzi rad da ne povlacimo iz neo svaki put
                if(!(await _redis.KeyExistsAsync(RedisKeyQuestions))) {
                    if (tempQuiz.Questions != null) {
                        foreach(QuestionDto question in tempQuiz.Questions)
                            await _redis.ListRightPushAsync(RedisKeyQuestions, _util.SerializeQuestion(question));
                        await _redis.KeyExpireAsync(RedisKeyQuestions, Duration.Questions);
                    } else throw new KviziramException(Msg.NoQuestions);
                }

                if (game.IsSearchable) {
                    GameDto gameDTO = new GameDto();
                    gameDTO.Created = game.Created.Value;
                    gameDTO.InviteCode = game.InviteCode;
                    gameDTO.HostName = _util.CallerAccountExists().Username;
                    gameDTO.QuizName = (tempQuiz.Name != null) ? tempQuiz.Name : string.Empty;
                    gameDTO.CategoryName = (tempQuiz.Category != null && tempQuiz.Category.Name != null) ? tempQuiz.Category.Name : string.Empty;
                    gameDTO.TrophyName = (tempQuiz.Achievement != null && tempQuiz.Achievement.Name != null) ? tempQuiz.Achievement.Name : string.Empty;

                    string gameDTOjson = _util.SerializeGameDto(gameDTO);
                    await _redis.SortedSetAddAsync(_util.RedisKeyPublicMatches, gameDTOjson, game.Created.Value.ToOADate());
                }
                
                await _redis.ListLeftPushAsync(_util.RedisKeyChat(game.InviteCode), Msg.ChatWelcome);
                await _redis.KeyExpireAsync(_util.RedisKeyChat(game.InviteCode), Duration.Chat);

                await _redis.StringSetAsync(_util.RedisKeyGame(game.InviteCode), _util.SerializeMatch(game), Duration.Game);
            } else throw new KviziramException(Msg.NoQuiz);
            return _util.DeserializeMatch(await _redis.StringGetAsync(_util.RedisKeyGame(game.InviteCode)));
        }
        throw new KviziramException(Msg.NoMatch);
    }

    public async Task<List<GameDto>?> GetPublicGamesAsync(FromToDate fromToDate, int skip, int limit, bool asc) {
        double fromDate = fromToDate.FromDate.ToOADate();
        double toDate = fromToDate.ToDate.ToOADate();
        Order order = (asc) ? Order.Ascending : Order.Descending;

        var gamesObj = await _redis.SortedSetRangeByScoreAsync(_util.RedisKeyPublicMatches, fromDate, toDate, Exclude.None, order, skip, limit);
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
        Match? match = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RedisKeyGame(inviteCode)));

        if (match != null && match.HostID != null && match.Created != null) {
            if (match.HostID == _util.CallerAccountExists().ID) {
                var playersList = await _redis.HashKeysAsync(_util.RedisKeyLobby(inviteCode));

                if (playersList != null) {
                    foreach(RedisValue playerGuid in playersList) {
                        await _redis.SortedSetAddAsync(_util.RedisKeyScores(inviteCode), playerGuid, 0.00);
                        await _redis.ListLeftPushAsync(_util.RedisKeyPlayedGames(playerGuid), inviteCode);
                    }

                    await _redis.KeyExpireAsync(_util.RedisKeyLobby(inviteCode), Duration.GameScore);
                    await _redis.StringSetAsync(_util.RedisKeyPlayersAnswered(inviteCode), 0, Duration.NumberOfAnswers);
                    
                    match.GameState = GameState.Playing;
                    await _redis.StringSetAsync(_util.RedisKeyGame(inviteCode), _util.SerializeMatch(match), Duration.Game);

                    double PublicGameKey = match.Created.Value.ToOADate();
                    await _redis.SortedSetRemoveRangeByScoreAsync(_util.RedisKeyPublicMatches, PublicGameKey, PublicGameKey);

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
        Match? game = _util.DeserializeMatch(await _redis.StringGetAsync(_util.RedisKeyGame(inviteCode)));
        if (game != null && game.Created != null && game.InviteCode != null && game.QuizID != null) {
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


    
}