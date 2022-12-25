using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Microsoft.AspNetCore.Http;

public class AccountService: IAccountService {
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

    #region Account Main Functions
    public async Task<AccountView> GetAccountViewAsync(Guid uID) {
        Account? account = await GetAccountQueryAsync(uID);
        if (account == null) 
            throw new KviziramException(Msg.NoAccount);
        return new AccountView(account);
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

    #region Account Helper Functions
    public async Task<Account?> GetAccountQueryAsync(Guid uID) {
        IEnumerable<Account?> query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.ID == uID)
            .Return(a => a.As<Account>()).ResultsAsync;

        return query.SingleOrDefault();
    }

    public async Task RequestRelationshipQueryAsync(Guid fuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)","(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Merge("(me)-[:RELATIONSHIP {Status: $prop}]-(them)")
                .WithParams(new {prop = RelationshipState.Pending})
                .ExecuteWithoutResultsAsync();
        } else
            throw new KviziramException(Msg.Unknown);        
    }

    public async Task AnswerRelationshipQueryAsync(Guid fuID, RelationshipState answer) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Set("r.Status = $prop")
                .WithParam("prop", answer)
                .ExecuteWithoutResultsAsync();
        } else
            throw new KviziramException(Msg.Unknown);        
    }

    public async Task RemoveRelationshipQueryAsync(Guid fuID) {
        if (_context.AccountCaller != null) {
            await _neo.Cypher
                .Match("(me:Account)-[r:RELATIONSHIP]-(them:Account)")
                .Where((Account me) => me.ID == _context.AccountCaller.ID)
                .AndWhere((Account them) => them.ID == fuID)
                .Delete("(r)")
                .ExecuteWithoutResultsAsync();
        } else
            throw new KviziramException(Msg.Unknown); 
    }

    #endregion

}