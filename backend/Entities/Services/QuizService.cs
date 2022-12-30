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
    public async Task<Quiz?> GetQuizAsync(Guid quID) {
        return await GetQuizQueryAsync(quID);
    }
    
    public async Task<Quiz> UpdateQuizAsync(Quiz updatedQuiz) {
        //lmao, tho nema smisla da modifikujes kviz nakon sto je neko vec igrao, rate-ovao i dobio trofej za stara pitanja :)
        await DeleteQuizQueryAsync(updatedQuiz.ID);
        return await CreateQuizAsync(updatedQuiz);
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
    
    public async Task<string> DeleteQuizAsync(Guid quID) {
        Quiz? quizExists = await GetQuizQueryAsync(quID);
        if (quizExists == null)
            throw new KviziramException(Msg.NoCategory);
        await DeleteQuizQueryAsync(quID);
        return ("Quiz: " + quID + Msg.Deleted);        
    }

    public async Task<List<AccountQuizRatingDto>?> GetQuizRatingsAsync(Guid quID) {
        return await GetQuizRatingsQueryAsync(quID);
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
                Quizzes = q.CollectAs<QuizDto>(),
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
    
    public async Task<string> ConnectQuizAchievementAsync(Guid quID, Guid? acuID) {
        await ConnectQuizAchievementQueryAsync(quID, acuID);
        return Msg.ConnectedQuizAchievement;
    }

    public async Task<string> DisconnectQuizAchievementAsync(Guid quID, Guid? acuID) {
        await DisconnectQuizAchievementQueryAsync(quID, acuID);
        return Msg.DisconnectedQuizAchievement;
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

    public async Task ConnectQuizAchievementQueryAsync(Guid quID, Guid? acuID) {
        await _neo.Cypher
            .Match("(q:Quiz)")
            .Where((Quiz q) => q.ID == quID)
            .Match("(ac:Achievement)")
            .Where((Achievement ac) => ac.ID == acuID)
            .Merge("(q)-[:AWARD]->(ac)")
            .ExecuteWithoutResultsAsync();
    }

    public async Task DisconnectQuizAchievementQueryAsync(Guid quID, Guid? acuID) {
        await _neo.Cypher
            .OptionalMatch("(q:Quiz)-[r:AWARD]->(ac:Achievement)")
            .Where((Quiz q) => q.ID == quID)
            .AndWhere((Achievement ac) => ac.ID == acuID)
            .Delete("r")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<Quiz?> GetQuizQueryAsync(Guid quID) {
        var query = await _neo.Cypher
            .OptionalMatch("(q:Quiz)")
            .Where((Quiz q) => q.ID == quID)
            .Match("(q)-[:HAS_QUESTIONS]->(qs:Questions)")
            .Match("(q)-[:IS_TYPE]->(c:Category)")
            .Match("(q)<-[:CREATOR]-(a:Account)")
            .OptionalMatch("(q)-[:AWARD]->(ac:Achievement)")
            .Return((q, qs, c, a, ac) => new {
                    Quiz = q.As<Quiz>(),
                    Questions = qs.As<QuestionPoco>(),
                    Category = c.As<Category>(),
                    Creator = a.As<AccountPoco>(),
                    Achievement = ac.As<Achievement>()
                })
            .ResultsAsync;
        var result = query.Single();
        result.Quiz.Questions = result.Questions.DeserializeInfo();
        result.Quiz.Category = result.Category;
        result.Quiz.Creator = result.Creator;
        result.Quiz.Achievement = result.Achievement;
        return result.Quiz;
    }

    public async Task<List<AccountQuizRatingDto>?> GetQuizRatingsQueryAsync(Guid quID) {
        var query = await _neo.Cypher
            .OptionalMatch("(q:Quiz)<-[r:RATING]-(a:Account)")
            .Where((Quiz q) => q.ID == quID) 
            .Return((a,r) => new { 
                Account = a.CollectAs<AccountPoco>(),
                Ratings = r.CollectAs<QuizRatingDto>()
            }).ResultsAsync;
        var result = query.FirstOrDefault();

        if (result != null && result.Account.Count() > 0) {
            List<AccountQuizRatingDto> resultList = new List<AccountQuizRatingDto>(); 
            for(int i = 0; i < result.Account.Count(); i++) {
                AccountQuizRatingDto obj = new AccountQuizRatingDto();

                obj.Account = result.Account.ElementAt(i);
                obj.Rating = result.Ratings.ElementAt(i);

                resultList.Add(obj);
            }
            return resultList;
        }        
        return null;
    }
    
    public async Task DeleteQuizQueryAsync(Guid quID) {
        var query = _neo.Cypher
            .Match("(q:Quiz)-[:HAS_QUESTIONS]->(qs:Questions)")
            .Where((Quiz q) => q.ID == quID)
            .DetachDelete("q")
            .Delete("qs");
        Console.WriteLine(query.Query.DebugQueryText);
        await query.ExecuteWithoutResultsAsync();
    }    
    #endregion
}