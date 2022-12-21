using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Microsoft.AspNetCore.Http;

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
    public async Task<string> login(string? authorization) {
        _util.AlreadyLogged();

        if (string.IsNullOrEmpty(authorization)) 
            throw new KviziramException(Msg.NoAuth);

        var loginInfo = decodeAuth(authorization);
        var checkAccount = await AccountEmailExists(loginInfo.email);

        if (checkAccount.account == null) throw new KviziramException(Msg.NoAuth);
        verifyPassword(loginInfo.password, checkAccount.account.Password);

        string newSID = "guest:" + BCrypt.Net.BCrypt.GenerateSalt() + ":id";
        AccountView accountView = new AccountView(checkAccount.account);

        await _redis.StringSetAsync(newSID, accountView.ToJsonString(), new TimeSpan(0, 60, 0));

        return newSID;
    }

    public async Task<bool> register(Account newAccount) {
        var checkAccount = await AccountEmailExists(newAccount.Email);
        if (checkAccount.exists)
            throw new KviziramException(Msg.UsedEmail);

        newAccount.ID = Guid.NewGuid();
        newAccount.Password = hashPassword(newAccount.Password);
        newAccount.isAdmin = false;
        
        await _neo.Cypher.Create("(a:Account $prop)").WithParam("prop", newAccount).ExecuteWithoutResultsAsync();

        return true;
    }
    #endregion

    #region Login/Register Helper Functions
    public (string email, string password) decodeAuth(string authorization) {
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
            return (true, query.FirstOrDefault<Account?>());

        return (false, null);
    }

    public string hashPassword(string pass) {
        return BCrypt.Net.BCrypt.HashPassword(pass);
    }

    public void verifyPassword(string passedPassword, string databasePassword) {
        bool verified = BCrypt.Net.BCrypt.Verify(passedPassword, databasePassword);
        if (verified == false) 
            throw new KviziramException(Msg.BadPassword);
    }
    #endregion
}