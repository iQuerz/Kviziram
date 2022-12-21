using StackExchange.Redis;
using Neo4jClient;
using System.Text.Json;


public class Utility
{
    private IDatabase _redis;
    private IGraphClient _neo;
    private KviziramContext _context;

    public Utility(KviziramContext context)
    {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
    }

    #region Caller functions
    public bool isStringSidValid(string sID)
    {
        return sID.Contains("account") || sID.Contains("guest");
    }

    public void CallerExists() {
        if (_context.SID == null)
            throw new KviziramException(Msg.NoSession);

        if (_redis.KeyExists(_context.SID)) 
            _context.Redis.GetDatabase().KeyExpire(_context.SID, new TimeSpan(0, 60, 0));
        else
            throw new KviziramException(Msg.NoSession);
    }

    public bool AlreadyLogged() {
        if (_context.SID != null)
            throw new KviziramException(Msg.AlreadyLoggedIn);
        return false;
    }

    public void SetAccountAndGuestCaller() {
        CallerExists();

        string accountGuestString = _redis.StringGet(_context.SID).ToString();
        if (accountGuestString.Contains("account")) 
            _context.AccountCaller = JsonSerializer.Deserialize<AccountView>(accountGuestString);
        else
            _context.GuestCaller = JsonSerializer.Deserialize<Guest>(accountGuestString);
    }
    #endregion

    



}
