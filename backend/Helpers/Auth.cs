using backend.Data;
public class Auth
{
    readonly RequestDelegate _next;

    public Auth(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context, KviziramContext _context) {
        string? sID = context.Request.Headers["SessionID"];
        if (!string.IsNullOrEmpty(sID))
            _context.caller = null;
        else 
            _context.caller = null;
        await _next(context);
    }
}
