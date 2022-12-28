using System.Net;
using System.Text.Json;

public class Auth
{
    readonly RequestDelegate _next;

    public Auth(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context, KviziramContext _context, Utility _util) {
        try {
            string? sID = context.Request.Headers["SessionID"];
            if (!string.IsNullOrEmpty(sID)) {
                string? account = (await _context.Redis.GetDatabase().StringGetAsync(_util.RedisKeyAccount(sID))).ToString();
                string? guest = (await _context.Redis.GetDatabase().StringGetAsync(_util.RedisKeyGuest(sID))).ToString();
                if (account != null) {
                    _context.AccountCaller = JsonSerializer.Deserialize<AccountPoco>(account);
                    _context.SID = _util.RedisKeyAccount(sID);
                } 
                else if (guest != null) {
                    _context.GuestCaller = JsonSerializer.Deserialize<Guest>(guest);
                    _context.SID = _util.RedisKeyGuest(sID);
                }
            } else {
                _context.SID = null;
            }
            await _next(context);
        } catch (Exception e) {
            await HandleExceptionAsync(context, e);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception e) {

        HttpResponse response = context.Response;
        response.StatusCode = (int) HttpStatusCode.InternalServerError;
        string message = Msg.Unknown;

        if (typeof(KviziramException).IsInstanceOfType(e)) {
            response.StatusCode = (int) HttpStatusCode.BadRequest;
            message = e.Message ?? "KviziramException: Unknown";
        }

        Console.WriteLine(e.StackTrace); //realno logger bi trebao ovde ali ide zivot
        await response.WriteAsJsonAsync(message);
    }


}
