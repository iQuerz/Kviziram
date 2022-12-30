using StackExchange.Redis;
using Neo4jClient;

public class QuizService: IQuizService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;

    private ICategoryService _category;
    private IAccountService _account;
    private IAchievementService _achievement;

    
    public QuizService(KviziramContext context, Utility utility, ICategoryService category, IAccountService account, IAchievementService achievement) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;

        _category = category;
        _account = account;
        _achievement = achievement;
    }

    #region Main Functions
    public async Task<Quiz?> GetQuizAsync(Guid uID) {
        var query = await GetQuizQueryAsync(uID);
        return query;
    }

    public async Task<List<QuizDto>?> SearchQuizzesAsync(QuizQuery quizQuery) {
        var finalQuery = _neo.Cypher.OptionalMatch("(q:Quiz)");

        if (quizQuery.CreatorID != null) finalQuery = finalQuery.Match("(q)<-[:CREATOR]-(a:Account)").Where((Account a) => a.ID == quizQuery.CreatorID);
        if (quizQuery.AchievementID != null) finalQuery = finalQuery.Match("(q)-[:AWARD]->(ac:Achievement)").Where((Achievement ac) => ac.ID == quizQuery.AchievementID);
        if (quizQuery.CategoryID != null) finalQuery = finalQuery.Match("(q)-[:IS_TYPE]->(c:Category)").Where((Category c) => c.ID == quizQuery.CategoryID);
        var query = finalQuery
            .OptionalMatch("(q)<-[r:RATING]-(:Account)")
            .With("q, avg(r.Rating) as rating")
            .Return((q, rating) => new {
                Quizzes = q.CollectAsDistinct<QuizDto>(),
                Ratings = rating.CollectAs<float>()
            });
        var result = (await query.ResultsAsync).Single();

        IEnumerable<QuizDto> tempList = result.Quizzes;
        IEnumerable<float> tempFloat = result.Ratings;
        //Ne znam dal ima bolji nacin za ovo
        int i, j;
        for( i = 0; i < tempFloat.Count(); i++)
            tempList.ElementAt(i).AvgRating = tempFloat.ElementAt(i);
        for( j = i; j < tempList.Count(); j++)
            tempList.ElementAt(j).AvgRating = 0;

        if (quizQuery.OrderAsc)
            return tempList.OrderBy(x => x.AvgRating).ToList();
        else
            return tempList.OrderByDescending(x => x.AvgRating).ToList();
    }

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

        if (newQuiz.AchievementID != null && _util.CallerAccountExists().isAdmin)
            await ConnectQuizAchievementQueryAsync(newQuiz.ID, newQuiz.AchievementID);

        return newQuiz;
    }
    #endregion

    #region Helper Functions
    public async Task<Quiz> CreateQuizQueryAsync(Quiz newQuiz) {
        if (_context.AccountCaller != null && newQuiz.CategoryID != null) {
            await _neo.Cypher
                .Match("(a:Account)")
                .Where((Account a) => a.ID == _context.AccountCaller.ID)
                .Merge("(a)-[:CREATOR]->(q:Quiz { ID: $id, Name: $name})")
                .WithParams(new { id = newQuiz.ID, name = newQuiz.Name})
                .With("q")
                .Match("(c:Category)")
                .Where((Category c) => c.ID == newQuiz.CategoryID)                
                .Merge("(q)-[:HAS_QUESTIONS]->(qs:Questions { ID: $idQS, Info: $propQS})")
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

    public async Task<Quiz?> GetQuizQueryAsync(Guid uID) {
        var query = await _neo.Cypher
            .OptionalMatch("(q:Quiz)")
            .Where((Quiz q) => q.ID == uID)
            .Match("(q)-[:HAS_QUESTIONS]->(qs:Questions)")
            .Return((q, qs) => new {
                    Quiz = q.As<Quiz>(),
                    Questions = qs.As<QuestionPoco>()
                })
            .ResultsAsync;
        var result = query.Single();
        result.Quiz.Questions = result.Questions.DeserializeInfo();
        return result.Quiz;
    }

    // public async Task<List<QuestionDto>> GetQuestionsQueryAsync(Guid quID) {
        
    // }
    #endregion
}