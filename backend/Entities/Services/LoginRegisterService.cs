using StackExchange.Redis;
using Neo4jClient;
using System.Text.Json;

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
        // _util.AlreadyLogged();
        // await Logout();

        if (string.IsNullOrEmpty(authorization)) 
            throw new KviziramException(Msg.NoAuth);

        var loginInfo = DecodeAuth(authorization);
        var checkAccount = await AccountEmailExistsQueryAsync(loginInfo.email);

        if (checkAccount.account == null) throw new KviziramException(Msg.NoAuth);
        VerifyPassword(loginInfo.password, checkAccount.account.Password);

        string newSID = BCrypt.Net.BCrypt.GenerateSalt();
        string accountKey = _util.RK_Account(newSID);

        await UpdateAccountStateQueryAsync(checkAccount.account.ID, PlayerState.Online);
        AccountPoco accountView = new AccountPoco(checkAccount.account);
        accountView.Status = PlayerState.Online;

        await _redis.StringSetAsync(accountKey, accountView.ToJsonString(), Duration.AccountLogin);

        return JsonSerializer.Serialize(newSID);
    }

    public async Task<Guid> Register(Account newAccount) {
        var checkAccount = await AccountEmailExistsQueryAsync(newAccount.Email);
        if (checkAccount.exists)
            throw new KviziramException(Msg.UsedEmail);

        newAccount.ID = Guid.NewGuid();
        newAccount.Password = HashPassword(newAccount.Password);
        newAccount.isAdmin = false;
        newAccount.Status = PlayerState.Offline;
        
        await _neo.Cypher.Create("(a:Account $prop)").WithParam("prop", newAccount).ExecuteWithoutResultsAsync();
        return newAccount.ID;
    }

    public async Task<string> LoginGuest(string username) {
        string newSID = BCrypt.Net.BCrypt.GenerateSalt();
        string guestKey = _util.RK_Guest(newSID);
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

    public async Task<string> Logout() {
        string sid = _util.GetRedisSID();
        if(sid != string.Empty) {
            Console.WriteLine(sid);
            if (await _redis.KeyDeleteAsync(sid)) {
                await UpdateAccountStateQueryAsync(_util.CallerAccountExists().ID, PlayerState.Offline);
                return Msg.LoggedOut;                
            }
        }
        return Msg.NoSession;
    }


    public async Task<(bool exists, Account? account)> AccountEmailExistsQueryAsync(string email) {
        IEnumerable<Account?>  query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.Email == email)
            .Return(a => a.As<Account>())
            .ResultsAsync;
        
        if (query.Any()) {
            return (true, query.SingleOrDefault<Account?>());
        }
        return (false, null);
    }

    public async Task UpdateAccountStateQueryAsync(Guid auID, PlayerState state) {
        var query = _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.ID == auID)
            .Set("a.Status = $prop")
            .WithParam("prop", state);
        Console.WriteLine(query.Query.DebugQueryText);
        await query.ExecuteWithoutResultsAsync();
            
    }
    #endregion
}