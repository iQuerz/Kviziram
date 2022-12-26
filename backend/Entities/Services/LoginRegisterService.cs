using StackExchange.Redis;
using Neo4jClient;

public class LoginRegisterService: ILoginRegisterService
{
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public LoginRegisterService(KviziramContext context, Utility utility) {
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    public async Task<string> Login(string? authorization) {
        _util.AlreadyLogged();

        if (string.IsNullOrEmpty(authorization)) 
            throw new KviziramException(Msg.NoAuth);

        var loginInfo = DecodeAuth(authorization);
        var checkAccount = await AccountEmailExistsQueryAsync(loginInfo.email);

        if (checkAccount.account == null) throw new KviziramException(Msg.NoAuth);
        VerifyPassword(loginInfo.password, checkAccount.account.Password);

        string newSID = BCrypt.Net.BCrypt.GenerateSalt();
        string accountKey = _util.RedisKeyAccount(newSID);

        AccountView accountView = new AccountView(checkAccount.account);

        await _redis.StringSetAsync(accountKey, accountView.ToJsonString(), new TimeSpan(12,0,0));

        return newSID;
    }

    public async Task<bool> Register(Account newAccount) {
        var checkAccount = await AccountEmailExistsQueryAsync(newAccount.Email);
        if (checkAccount.exists)
            throw new KviziramException(Msg.UsedEmail);

        newAccount.ID = Guid.NewGuid();
        newAccount.Password = HashPassword(newAccount.Password);
        newAccount.isAdmin = false;
        
        await _neo.Cypher.Create("(a:Account $prop)").WithParam("prop", newAccount).ExecuteWithoutResultsAsync();
        return true;
    }

    public async Task<string> LoginGuest(string username) {
        string newSID = BCrypt.Net.BCrypt.GenerateSalt();
        string guestKey = _util.RedisKeyGuest(newSID);
        Guest? guest = new Guest(Guid.NewGuid(), username);

        await _redis.StringSetAsync(guestKey, guest.ToJsonString(), new TimeSpan(6,0,0));        
        return newSID;
    }
    #endregion

    #region Helper Functions
    public (string email, string password) DecodeAuth(string authorization) {
        string[] EmailPasswordArray = authorization.Split(":", 2);
        return (EmailPasswordArray[0], EmailPasswordArray[1]);
    }

    public string HashPassword(string pass) {
        return BCrypt.Net.BCrypt.HashPassword(pass);
    }

    public void VerifyPassword(string passedPassword, string databasePassword) {
        bool verified = BCrypt.Net.BCrypt.Verify(passedPassword, databasePassword);
        if (verified == false) 
            throw new KviziramException(Msg.BadPassword);
    }

    public async Task<(bool exists, Account? account)> AccountEmailExistsQueryAsync(string email) {
        IEnumerable<Account?>  query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.Email == email)
            .Return(a => a.As<Account>())
            .ResultsAsync;
        
        if (query.Any())
            return (true, query.SingleOrDefault<Account?>());

        return (false, null);
    }
    #endregion
}