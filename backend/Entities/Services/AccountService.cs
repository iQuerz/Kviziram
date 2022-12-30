using StackExchange.Redis;
using Neo4jClient;

public class AccountService: IAccountService 
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public AccountService(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    public async Task<AccountPoco> GetAccountAsync(Guid? uID) {
        Account? account = await GetAccountQueryAsync(uID);
        if (account == null) 
            throw new KviziramException(Msg.NoAccount);
        return new AccountPoco(account);
    }

    public async Task<List<AccountPoco>> GetFriendsAsync(RelationshipState rState) {
        if (_util.IsCaller().account) {
            List<AccountPoco>? listFriends = await GetFriendsQueryAsync(rState);
            if (listFriends == null) throw new KviziramException(Msg.NoAnything);
            return listFriends;
        } 
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<AccountPoco>> GetMutualFriendsAsync(RelationshipState rState) {
        if (_util.IsCaller().account) {
            List<AccountPoco>? listRecommendedFriends = await GetFriendsQueryAsync(rState);

            //Friends of Friends Query + Non Friend Accounts from played Games
            if (listRecommendedFriends == null) throw new KviziramException(Msg.NoAnything);
            return listRecommendedFriends;
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

    public async Task<bool> AddRatingAsync(Guid quID, QuizRatingDto newRating) {
        if (_util.IsCaller().account) {
            await AddRatingQueryAsync(quID, newRating);
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
    #endregion
    
    #region Helper Functions
    public async Task<Account?> GetAccountQueryAsync(Guid? uID) {
        IEnumerable<Account?> query = await _neo.Cypher
            .OptionalMatch("(a:Account)")
            .Where((Account a) => a.ID == uID)
            .Return(a => a.As<Account>()).ResultsAsync;

        return query.SingleOrDefault();
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
                .Match("(me:Account)-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Delete("(r)")
                .ExecuteWithoutResultsAsync();
        }        
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

    #endregion

}