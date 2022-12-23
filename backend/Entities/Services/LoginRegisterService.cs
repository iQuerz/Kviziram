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

    #region Login/Register Main Functions
    public async Task<string> Login(string? authorization) {
        _util.AlreadyLogged();

        if (string.IsNullOrEmpty(authorization)) 
            throw new KviziramException(Msg.NoAuth);

        var loginInfo = DecodeAuth(authorization);
        var checkAccount = await AccountEmailExists(loginInfo.email);

        if (checkAccount.account == null) throw new KviziramException(Msg.NoAuth);
        VerifyPassword(loginInfo.password, checkAccount.account.Password);

        string newSID = "account:" + BCrypt.Net.BCrypt.GenerateSalt() + ":id";
        AccountView accountView = new AccountView(checkAccount.account);

        await _redis.StringSetAsync(newSID, accountView.ToJsonString(), new TimeSpan(12,0,0));

        return newSID;
    }

    public async Task<bool> Register(Account newAccount) {
        var checkAccount = await AccountEmailExists(newAccount.Email);
        if (checkAccount.exists)
            throw new KviziramException(Msg.UsedEmail);

        newAccount.ID = Guid.NewGuid();
        newAccount.Password = HashPassword(newAccount.Password);
        newAccount.isAdmin = false;
        
        await _neo.Cypher.Create("(a:Account $prop)").WithParam("prop", newAccount).ExecuteWithoutResultsAsync();
        return true;
    }

    public async Task<string> LoginGuest(string username, Guid? uID = null) {
        string newSID = "guest:" + BCrypt.Net.BCrypt.GenerateSalt() + ":id";
        Guest? guest;
        
        if (uID == null) {
            guest = new Guest();
            guest.ID = Guid.NewGuid();
            guest.Username = username;
        } else {
            // IEnumerable<Guest?>  query = await 
            Console.WriteLine("I am here: " + uID);
            IEnumerable<Guest?> query = await _neo.Cypher
                .Match("(g:Guest)")
                .Where((Guest g) => g.ID == uID)
                .Return(g => g.As<Guest>()).ResultsAsync;

            guest = query.FirstOrDefault<Guest?>();
            if (guest != null) {
                guest.Username = username;

                await _neo.Cypher
                    .Match("(g:Guest)")
                    .Where((Guest g) => g.ID == uID)
                    .Set($"g.Username = '{username}'")
                    .ExecuteWithoutResultsAsync();
            }
        }

        if (guest == null) 
            throw new KviziramException(Msg.Unknown);
        await _redis.StringSetAsync(newSID, guest.ToJsonString(), new TimeSpan(6,0,0));
        
        return newSID;
    }

    public async Task<bool> RegisterGuest(Guest newGuest) {
        await _neo.Cypher.Create("(g:Guest $prop)").WithParam("prop", newGuest).ExecuteWithoutResultsAsync();
        return true;
    }

    #endregion

    #region Login/Register Helper Functions
    public (string email, string password) DecodeAuth(string authorization) {
        string[] EmailPasswordArray = authorization.Split(":", 2);
        return (EmailPasswordArray[0], EmailPasswordArray[1]);
    }

    public async Task<(bool exists, Account? account)> AccountEmailExists(string email) {
        IEnumerable<Account?>  query = await _neo.Cypher
            .Match("(a:Account)")
            .Where((Account a) => a.Email == email)
            .Return(a => a.As<Account>())
            .ResultsAsync;
        
        if (query.Any())
            return (true, query.SingleOrDefault<Account?>());

        return (false, null);
    }

    public string HashPassword(string pass) {
        return BCrypt.Net.BCrypt.HashPassword(pass);
    }

    public void VerifyPassword(string passedPassword, string databasePassword) {
        bool verified = BCrypt.Net.BCrypt.Verify(passedPassword, databasePassword);
        if (verified == false) 
            throw new KviziramException(Msg.BadPassword);
    }
    #endregion
}