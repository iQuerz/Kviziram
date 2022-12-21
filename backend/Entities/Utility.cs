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

    public async Task<bool> CallerExists()
    {
        if (await _redis.KeyExistsAsync(_context.SID)) {
            await _context.Redis.GetDatabase().KeyExpireAsync(_context.SID, new TimeSpan(0, 30, 0));
            return true;
        }
        return false;
    }

    public bool AlreadyLogged() {
        if (_context.SID != null)
            throw new KviziramException(Msg.AlreadyLoggedIn);
        return false;
    }

    public async Task<AccountView?> isAccountCaller()
    {
        if (await _redis.KeyExistsAsync(_context.SID)) {
            var accountString = await _redis.StringGetAsync(_context.SID);
            AccountView? accountView = JsonSerializer.Deserialize<AccountView>(accountString);
            return accountView;
        } 
        throw new KviziramException(Msg.NoSession);
    }

    public Guest isGuestCaller()
    {
        //TODO: Iskoristiti _contex.SID da se povuku informacije o guest caller-u iz redisa
        Guest caller = new Guest();
        return caller;
    }
    #endregion

    



}
