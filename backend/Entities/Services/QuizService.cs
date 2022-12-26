using StackExchange.Redis;
using Neo4jClient;

public class QuizService: IQuizService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public QuizService(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    #endregion

    #region Helper Functions
    #endregion
}