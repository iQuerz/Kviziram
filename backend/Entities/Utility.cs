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

    #region Caller Functions
    public bool isStringSidValid(string sID)
    {
        return sID.Contains("account") || sID.Contains("guest");
    }

    public void CallerExists() {
        if (_context.SID == null)
            throw new KviziramException(Msg.NoSession);

        if (_context.AccountCaller == null && _context.GuestCaller == null)
            throw new KviziramException(Msg.NoAuth);
    }

    public AccountPoco CallerAccountExists() {
        if (_context.AccountCaller == null)
            throw new KviziramException(Msg.NoAuth);
        return _context.AccountCaller;
    }

    public bool IsCallerAccountAdmin() {
        if (!CallerAccountExists().isAdmin) 
            throw new KviziramException(Msg.NoAccess);
        else return true;
    }

    public Guest CallerGuestExists() {
        if (_context.GuestCaller == null)
            throw new KviziramException(Msg.NoAuth);
        return _context.GuestCaller;
    }

    public (bool account, bool guest) IsCaller() {
        bool account = (_context.AccountCaller == null) ? false : true;
        bool guest = (_context.GuestCaller == null) ? false : true;
        return (account, guest);
    }

    public bool AlreadyLogged() {
        if (_context.SID != null)
            throw new KviziramException(Msg.AlreadyLoggedIn);
        return false;
    }
    #endregion

    #region Redis Key Functions
    public string RedisKeyGuest(string key) { return "guest:" + key + ":id"; }
    public string RedisKeyAccount(string key) { return "account:" + key + ":id"; }
    #endregion

    



}
