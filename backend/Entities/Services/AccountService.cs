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
    public async Task<AccountView> GetAccountViewAsync(Guid uID) {
        Account? account = await GetAccountQueryAsync(uID);
        if (account == null) 
            throw new KviziramException(Msg.NoAccount);
        return new AccountView(account);
    }

    public async Task<List<AccountView>> GetFriendsAsync(RelationshipState rState) {
        if (_util.IsCaller().account) {
            List<AccountView>? listFriends = await GetFriendsQueryAsync(rState);
            if (listFriends == null) throw new KviziramException(Msg.NoAnything);
            return listFriends;
        } 
        throw new KviziramException(Msg.NoAccess);
    }

    public async Task<List<AccountView>> GetMutualFriendsAsync(RelationshipState rState) {
        if (_util.IsCaller().account) {
            List<AccountView>? listRecommendedFriends = await GetFriendsQueryAsync(rState);

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
    #endregion

    #region Helper Functions
    public async Task<Account?> GetAccountQueryAsync(Guid uID) {
        IEnumerable<Account?> query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.ID == uID)
            .Return(a => a.As<Account>()).ResultsAsync;

        return query.SingleOrDefault();
    }

    public async Task<List<AccountView>?> GetFriendsQueryAsync(RelationshipState rState) {
        if (_context.AccountCaller != null) {            
            IEnumerable<AccountView> listAccounts;
            if (rState == RelationshipState.Blocked) 
                listAccounts = await _neo.Cypher
                    .Match("(me:Account)-[r:RELATIONSHIP]->(a:Account)")
                    .Where((Account me) => me.ID == _context.AccountCaller.ID)
                    .AndWhere((RelationRelationship r) => r.Status.ToString() == rState.ToString())
                    .Return(a => a.As<AccountView>()).ResultsAsync;
            else {
                listAccounts = await _neo.Cypher
                    .Match("(me:Account)-[r:RELATIONSHIP]-(a:Account)")
                    .Where((Account me) => me.ID == _context.AccountCaller.ID)
                    .AndWhere((RelationRelationship r) => r.Status.ToString() == rState.ToString())
                    .Return(a => a.As<AccountView>()).ResultsAsync;
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

    #endregion

}