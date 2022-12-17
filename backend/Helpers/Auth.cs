public class Auth
{
    readonly RequestDelegate _next;

    public Auth(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context, KviziramContext _context) {
        string? sID = context.Request.Headers["SessionID"];
        if (!string.IsNullOrEmpty(sID))
            _context.Caller = null;
        else 
            _context.Caller = null;
        await _next(context);
    }
}
