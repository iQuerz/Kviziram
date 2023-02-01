using StackExchange.Redis;
using Neo4jClient;

public class AccountService: IAccountService 
{
    KviziramContext _context;
    IDatabase _redis;
    IGraphClient _neo;
    Utility _util;

    IAdService _ad;
    ICategoryService _category;
    
    
    public AccountService(KviziramContext context, IAdService ad, ICategoryService category, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _ad = ad;
        _category = category;
    }

    #region Main Functions
    public async Task<AccountPoco> GetAccountAsync(Guid? uID) {
        Account? account = await GetAccountQueryAsync(uID);
        if (account == null) 
            throw new KviziramException(Msg.NoAccount);
        AccountPoco accountPoco = new AccountPoco(account);
        accountPoco.isFriend = await GetRelationshipQueryAsync(uID);
        return accountPoco;
    }

    public async Task<List<AccountPoco>> GetAllAccountsAsync() {
        var res = await _neo.Cypher
            .Match("(a:Account)")
            .Return(a => a.As<AccountPoco>())
            .ResultsAsync;
        return res.ToList();
    }

    public async Task<List<AccountPoco>?> GetAccountsByUsername(string username) {
        var query = _neo.Cypher
            .Match("(a:Account)")
            .Where("toLower(a.Username) = toLower($prop)")
            .WithParam("prop", username)
            .Return(a => a.As<AccountPoco>());
        Console.WriteLine(query.Query.DebugQueryText);
        return (await query.ResultsAsync).ToList();
    }

    public async Task<AccountPoco?> GetMyAccount() {
        return _util.DeserializeAccountPoco(await _redis.StringGetAsync(_context.SID));
    }

    public async Task<List<AccountPoco>> GetFriendsAsync(RelationshipState rState) {
        if (_util.IsCaller().account) {
            List<AccountPoco>? listFriends = await GetFriendsQueryAsync(rState);
            if (listFriends == null) throw new KviziramException(Msg.NoAnything);
            return listFriends;
        } 
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> RequestRelationshipAsync(Guid fuID) {
        if (_util.IsCaller().account) {
                await RequestRelationshipQueryAsync(fuID);
                return Msg.RequestSent;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> AnswerRelationshipAsync(Guid fuID, RelationshipState answer) {
        if (_util.IsCaller().account) {
            if (answer == RelationshipState.Blocked)
                await BlockRelationshipAsync(fuID);
            else
                await AnswerRelationshipQueryAsync(fuID, answer);
            return (Msg.RequestAnswer + answer);
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> RemoveRelationshipAsync(Guid fuID) {
        if (_util.IsCaller().account) {
            await RemoveRelationshipQueryAsync(fuID);
            return Msg.RelationshipRemove;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> BlockRelationshipAsync(Guid fuID) {
        if (_util.IsCaller().account) {
            await RemoveRelationshipAsync(fuID);
            await BlockRelationshipQueryAsync(fuID);
            return Msg.AccountBlocked;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<QuizRatingDto?> GetRatingAsync(Guid quID) {
        if (_util.IsCaller().account) {
            QuizRatingDto? quizRating = await GetRatingQueryAsync(quID);
            return quizRating;
        }
        throw new KviziramException(Msg.NoAccess);
    }


    public async Task<bool> AddRatingAsync(Guid quID, QuizRatingDto newRating) {
        if (_util.IsCaller().account) {
            await AddRatingQueryAsync(quID, newRating);
            return true;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<bool> UpdateRatingAsync(Guid quID, QuizRatingDto updatedRating) {
        if (_util.IsCaller().account) {
            await UpdateRatingQueryAsync(quID, updatedRating);
            return true;
        }
        throw new KviziramException(Msg.NoAccess);

    }   


    public async Task<bool> RemoveRatingAsync(Guid quID) {
        if (_util.IsCaller().account) {
            await RemoveRatingQueryAsync(quID);
            return true;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<Category>?> GetPreferredCategoriesAsync() {
        if (_util.IsCaller().account) {
            return await GetPreferredCategoriesQueryAsync();
        }
        throw new KviziramException(Msg.NoAccess);
    }


    public async Task<string> SetPreferredCategoryAsync (List<Guid> categoryGuids) {
        if (_context.AccountCaller != null) {
            await SetPreferredCategoryQueryAsync(categoryGuids);
            //Moze bolje but this will do za postavljanje ads
            List<Ad>? adsList = new List<Ad>();
            foreach(Guid categoryuID in categoryGuids) {
                List<Ad>? tempList = await _category.GetCategoryAdsAsync(categoryuID);
                if (tempList != null)
                    adsList.AddRange(tempList);
            }
            adsList = adsList.DistinctBy(ad => ad.ID).ToList();
            List<Guid> account = new List<Guid>();
            account.Add(_context.AccountCaller.ID);
            foreach(Ad ad in adsList)
                await _ad.ConnectAdAccountAsync(ad.ID, account);

            return Msg.PreferredCategoriesSet;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> RemovePreferredCategoryAsync(Guid cuID) {
        if (_util.IsCaller().account) {
            await RemovePreferredCategoryQueryAsync(cuID);
            return Msg.PreferredCategoriesSet;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<string> SetUpdateAchievementAsync(Guid auID, Guid acuID) {
        await SetUpdateAchievementQueryAsync(auID, acuID);
        return Msg.AchievementSetUpdated;
    }

    public async Task<List<Achievement>?> GetAccountAchievementsAsync(Guid auID) {
        if (_util.IsCaller().account) {
            return await GetAccountAchievementsQueryAsync(auID);;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<Ad?> GetRecommendedAdsAsync() {
        return await GetRecommendedAdsQueryAsync();
    }

    public async Task<List<AccountPoco>?> GetRecommendedFriendsAsync() {
        if (_context.AccountCaller != null) {
        List<AccountPoco> recommended = new List<AccountPoco>();
        var fof = await GetFriendsOfFriendsAsync(_context.AccountCaller.ID);
        var players = await RecommendedPlayersFromMatchAsync(_context.AccountCaller.ID);

        if (fof != null) recommended.AddRange(fof);
        if (players != null) recommended.AddRange(players);

        return recommended.DistinctBy(a => a.ID).ToList();
        }
        return null;
    }

    public async Task<List<AccountPoco>?> GetFriendsOfFriendsAsync(Guid auID) {
        return await GetFriendsOfFriendsQueryAsync(auID);
    }

    public async Task<List<AccountPoco>?> RecommendedPlayersFromMatchAsync(Guid auID) {
        return await RecommendedPlayersFromMatchQueryAsync(auID);
    }

    public async Task<List<Quiz>?> GetRecommendedQuizzesAsync() {
        return await GetRecommendedQuizzesQueryAsync();
    }

    public async Task<bool> AccountExistsAsync(Guid uID) {
        Account? acc = await GetAccountQueryAsync(uID);
        return (acc != null) ? true : false;
    }

    #endregion
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Helper Functions
    public async Task<Account?> GetAccountQueryAsync(Guid? uID) {
        IEnumerable<Account?> query = await _neo.Cypher
            .OptionalMatch("(a:Account)")
            .Where((Account a) => a.ID == uID)
            .Return(a => a.As<Account>()).ResultsAsync;

        return query.SingleOrDefault();
    }

    public async Task<RelationshipState?> GetRelationshipQueryAsync(Guid? uID) {
        if (_context.AccountCaller != null) { 
            var query = await _neo.Cypher
                .OptionalMatch("(me:Account)-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == uID)
                .Return(r => r.As<RelationshipDto?>())
                .ResultsAsync;
            RelationshipDto? result = query.Single();
            if (result != null) return result.Status;
        }
        return null;
    }

    public async Task<List<AccountPoco>?> GetFriendsQueryAsync(RelationshipState rState) {
        if (_context.AccountCaller != null) {          
            IEnumerable<AccountPoco> listAccounts;
            if (rState == RelationshipState.Blocked)                
                listAccounts = await _neo.Cypher
                    .Match("(me:Account)-[r:RELATIONSHIP]->(a:Account)")
                    .Where((Account me) => me.ID == _context.AccountCaller.ID)
                    .AndWhere((RelationshipDto r) => r.Status.ToString() == rState.ToString())
                    .Return(a => a.As<AccountPoco>()).ResultsAsync;
            else {
                listAccounts = await _neo.Cypher
                    .Match("(me:Account)-[r:RELATIONSHIP]-(a:Account)")
                    .Where((Account me) => me.ID == _context.AccountCaller.ID)
                    .AndWhere((RelationshipDto r) => r.Status.ToString() == rState.ToString())
                    .Return(a => a.As<AccountPoco>()).ResultsAsync;
            }
            return listAccounts.ToList();
        }
        return null;
    }

    public async Task RequestRelationshipQueryAsync(Guid fuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)","(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .AndWhere("NOT ((me)-[:RELATIONSHIP]-(them))")
                .Merge("(me)-[:RELATIONSHIP {Status: $prop}]->(them)")
                .WithParams(new {prop = RelationshipState.Pending})
                .ExecuteWithoutResultsAsync();  
        }    
    }

    public async Task AnswerRelationshipQueryAsync(Guid fuID, RelationshipState answer) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)<-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Set("r.Status = $prop")
                .WithParam("prop", answer)
                .ExecuteWithoutResultsAsync(); 
        }            
    }

    public async Task RemoveRelationshipQueryAsync(Guid fuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .OptionalMatch("(me:Account)-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Delete("(r)")
                .ExecuteWithoutResultsAsync();
        }        
    }

    public async Task BlockRelationshipQueryAsync(Guid fuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .Match("(them:Account)")
                .Where((Account them) => them.ID == fuID)
                .Merge("(me)-[r:RELATIONSHIP]->(them)")
                .Set("r.Status = $prop")
                .WithParam("prop", RelationshipState.Blocked.ToString())
                .ExecuteWithoutResultsAsync(); 
        }
    }

    public async Task<QuizRatingDto?> GetRatingQueryAsync(Guid quID) {
        if (_context.AccountCaller != null) {
            var result = await _neo.Cypher
                .OptionalMatch("(me:Account)-[r:RATING]->(q:Quiz)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((QuizDto q) => q.ID == quID)
                .Return(r => r.As<QuizRatingDto?>())
                .ResultsAsync;
            return result.SingleOrDefault();
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task AddRatingQueryAsync(Guid quID, QuizRatingDto newRating) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .Match("(q:Quiz)")
                .Where((QuizDto q) => q.ID == quID)
                .Merge("(me)-[:RATING { Rating: $rating, Comment: $comment }]->(q)")
                .WithParams( new { rating = newRating.Rating, comment = newRating.Comment })
                .ExecuteWithoutResultsAsync();
        }
    }

    public async Task UpdateRatingQueryAsync(Guid quID, QuizRatingDto updatedRating) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)-[r:RATING]->(q:Quiz)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((QuizDto q) => q.ID == quID)
                .Set("r.Rating = $rating, r.Comment = $comment")
                .WithParams( new { rating = updatedRating.Rating, comment = updatedRating.Comment})
                .ExecuteWithoutResultsAsync();
        }
    }


    public async Task RemoveRatingQueryAsync(Guid quID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .Match("(q:Quiz)")
                .Where((QuizDto q) => q.ID == quID)
                .Merge("(me)-[r:RATING]->(q)")
                .Delete("r")
                .ExecuteWithoutResultsAsync();
        }
    }

    public async Task<List<Category>?> GetPreferredCategoriesQueryAsync() {
        if (_context.AccountCaller != null) {
            var query = await _neo.Cypher
                .OptionalMatch("(a:Account)-[r:PREFERS]->(c:Category)")
                .Where((Account a) => a.ID == _context.AccountCaller.ID)
                .Return( c => c.As<Category>())
                .ResultsAsync;
            return query.ToList();
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task SetPreferredCategoryQueryAsync (List<Guid> categoryGuids) {
        if (_context.AccountCaller != null) {
            string categoryList = _util.ListOfGuidsToString(categoryGuids);
            var query = _neo.Cypher
                .Match("(a:Account)") 
                .Where((Account a) => a.ID == _context.AccountCaller.ID)
                .Unwind(categoryGuids, "categoryID")
                .OptionalMatch("(c:Category)")
                .Where("c.ID = categoryID")
                .With("Collect(c) AS categories, a AS account")
                .ForEach("(category IN categories| MERGE (account)-[r:PREFERS]->(category) ON CREATE SET r.Weight = 1 ON MATCH SET r.Weight = r.Weight + 1)");
            await query.ExecuteWithoutResultsAsync();
        }
    }

    public async Task RemovePreferredCategoryQueryAsync(Guid cuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)-[r:PREFERS]->(c:Category)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Category c) => c.ID == cuID)
                .Delete("(r)")
                .ExecuteWithoutResultsAsync();
        }
    }

    public async Task SetUpdateAchievementQueryAsync(Guid auID, Guid acuID) {
            var query = _neo.Cypher
                .Match("(a:Account)") 
                .Where((Account a) => a.ID == auID)
                .Match("(ac:Achievement)")
                .Where((Achievement ac) => ac.ID == acuID)
                .Merge("(a)-[r:ACHIEVED]->(ac)")
                .OnCreate()
                .Set("r.Progress = 1")
                .OnMatch()
                .Set("r.Progress = r.Progress + 1");
            await query.ExecuteWithoutResultsAsync();
    }

    public async Task<List<Achievement>?> GetAccountAchievementsQueryAsync(Guid auID) {
        var query = _neo.Cypher
            .Match("(a:Account)") 
            .Where((Account a) => a.ID == auID)
            .OptionalMatch("(a)-[r:ACHIEVED]->(ac:Achievement)")
            .Return((ac,r) => new {
                Achievements = ac.CollectAs<Achievement>(),
                AchievementProgress = r.CollectAs<AchievedDto>()
            });
        var result = (await query.ResultsAsync).Single();
        if (result.Achievements.Count() != 0) {
            for(int i = 0; i < result.Achievements.Count(); i++) {
                result.Achievements.ElementAt(i).Progress = result.AchievementProgress.ElementAt(i).Progress;
            }
            return result.Achievements.ToList();
        }
        return null;
    }

    public async Task<Ad?> GetRecommendedAdsQueryAsync() {
        if (_context.AccountCaller != null) {
            var query = _neo.Cypher
                .Match("(a:Account)<-[r:AD_ACCOUNT]-(ad:Ad)-[p:AD_CATEGORY]->(c:Category)")
                .Where((Account a) => a.ID == _context.AccountCaller.ID)
                .AndWhere("r.Blocked = $prop").WithParam("prop", false)
                .With("ad, p")
                .OrderByDescending("p.Paid")
                .ReturnDistinct((ad) => ad.As<Ad>())
                .Limit(1);
            Console.WriteLine(query.Query.DebugQueryText);
            return (await query.ResultsAsync).SingleOrDefault();
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<AccountPoco>?> GetFriendsOfFriendsQueryAsync(Guid auID) {
        if (_context.AccountCaller != null) {
            var query = _neo.Cypher
                .Match("(a:Account)-[r:RELATIONSHIP]-(f:Account)-[p:RELATIONSHIP]-(fof:Account)")
                .Where((AccountPoco a) => a.ID == auID)
                .AndWhere("NOT (a:Account)-[:RELATIONSHIP]-(fof:Account)")
                .AndWhere("r.Status = 'Friend'")
                .ReturnDistinct( fof => fof.As<AccountPoco>());
            Console.WriteLine(query.Query.DebugQueryText);
            return (await query.ResultsAsync).ToList();
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<AccountPoco>?> RecommendedPlayersFromMatchQueryAsync(Guid auID) {
        if (_context.AccountCaller != null) {
            var query = _neo.Cypher
                .Match("(a:Account)-[:PARTICIPATED_IN*2]-(player:Account)")
                .Where((AccountPoco a) => a.ID == auID)
                .AndWhere("NOT (a:Account)-[:RELATIONSHIP]-(player:Account)")
                .ReturnDistinct( player => player.As<AccountPoco>());
            Console.WriteLine(query.Query.DebugQueryText);
            return (await query.ResultsAsync).ToList();
        }
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<Quiz>?> GetRecommendedQuizzesQueryAsync() {
        if (_context.AccountCaller != null) {
            List<Category>? categories = await GetPreferredCategoriesAsync();
            if (categories != null) {
                List<Guid> categoryGuids = categories.Select(c => c.ID).ToList();
                var query = _neo.Cypher
                    .Match("(a:Account)")
                    .Where((Account a) => a.ID == _context.AccountCaller.ID)
                    .With("a")
                    .Unwind(categoryGuids, "cuID")
                    .Match("(c:Category)<-[r:IS_TYPE]-(q:Quiz)")
                    .Where("c.ID = cuID")
                    .AndWhere("NOT (a)-[:PARTICIPATED_IN]->(:Match)-[:USED]->(q)")
                    .Return( q => q.As<Quiz>());
                Console.WriteLine(query.Query.DebugQueryText);
                return (await query.ResultsAsync).ToList();
            }
            return null;
        }
        throw new KviziramException(Msg.NoAccess);
    }

    #endregion

}