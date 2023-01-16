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
                
                if (game.IsSearchable)
                    await _redis.SortedSetAddAsync(_util.PublicMatches, game.InviteCode, game.Created.Value.ToOADate());
                
                await _redis.ListLeftPushAsync(_util.RedisKeyChat(game.InviteCode), "Welcome to the game boiii");
                await _redis.StringSetAsync(_util.RedisKeyGame(game.InviteCode), _util.SerializeMatch(game), Duration.Game);
            } else throw new KviziramException(Msg.NoQuiz);
            return game;
        }
        throw new KviziramException(Msg.NoMatch);
    }

    
}
