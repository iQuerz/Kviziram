using StackExchange.Redis;
using Neo4jClient;

public class QuizService: IQuizService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    private ICategoryService _category;
    
    public QuizService(KviziramContext context, Utility utility, ICategoryService category) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _category = category;
    }

    #region Main Functions
    public async Task<Quiz> CreateQuizAsync(Quiz newQuiz) {
        if (newQuiz.Category != null) {
            Category? categoryCheck = await _category.GetCategoryAsync(newQuiz.Category.ID);
            if (categoryCheck == null) throw new KviziramException(Msg.QuizNoCategory);        
        }

        newQuiz.CreatorID = _util.CallerAccountExists().ID;
        newQuiz.ID = Guid.NewGuid();

        if (newQuiz.Questions != null) {
            newQuiz.QuestionsID = Guid.NewGuid();
            await CreateQuizQueryAsync(newQuiz);
        }

        if (newQuiz.AchievementID != null)
            await ConnectQuizAchievementQueryAsync(newQuiz.ID, newQuiz.AchievementID);

        return newQuiz;
    }
    #endregion

    #region Helper Functions
    public async Task<Quiz> CreateQuizQueryAsync(Quiz newQuiz) {
        if (_context.AccountCaller != null && newQuiz.Category != null) {
            QuizPoco newQuizPoco = new QuizPoco(newQuiz);
            await _neo.Cypher
                .Match("(a:Account)")
                .Where((Account a) => a.ID == _context.AccountCaller.ID)
                .Merge("(a)-[:CREATOR]->(q:Quiz { ID: $id, Name: $name})")
                .WithParams(new { id = newQuizPoco.ID, name = newQuizPoco.Name})
                .With("q")
                .Match("(c:Category)")
                .Where((Category c) => c.ID == newQuiz.Category.ID)                
                .Merge("(q)-[:HAS_QUESTIONS]->(qs:Questions { ID: $idQS, List: $propQS})")
                .WithParams( new {idQS = newQuiz.QuestionsID, propQS = newQuiz.QuestionsToJsonString()})
                .Merge("(q)-[:IS_TYPE]->(c)")
                .ExecuteWithoutResultsAsync();
            return newQuiz;            
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<bool> ConnectQuizAchievementQueryAsync(Guid quID, Guid? acuID) {
        await _neo.Cypher
            .Match("(q:Quiz)")
            .Where((Quiz q) => q.ID == quID)
            .Match("(ac:Achievement)")
            .Where((Achievement ac) => ac.ID == acuID)
            .Merge("(q)-[:AWARD]->(ac)")
            .ExecuteWithoutResultsAsync();
        return true;
    }
    #endregion
}