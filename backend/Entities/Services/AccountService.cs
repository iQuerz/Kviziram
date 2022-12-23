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

    public async Task<AccountView> GetAccountViewAsync(Guid uid) {
        IEnumerable<Account?> query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.ID == uid)
            .Return(a => a.As<Account>()).ResultsAsync;

        Account? account = query.SingleOrDefault();
        if (account != null) return new AccountView(account);

        throw new KviziramException(Msg.NoAccount);
    }

    public async Task<RelationshipState> RequestRelationshipAsync(Guid fuid) {
        if (_context.AccountCaller != null) {

        }
        await Task.Delay(5);
        throw new KviziramException(Msg.NoAccess);
    }

}