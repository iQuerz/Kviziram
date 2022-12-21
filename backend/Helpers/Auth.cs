using System.Net;

public class Auth
{
    readonly RequestDelegate _next;

    public Auth(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context, KviziramContext _context, Utility _util) {
        try {
            string? sID = context.Request.Headers["SessionID"];
            if (!string.IsNullOrEmpty(sID) && _util.isStringSidValid(sID))
                _context.SID = sID;
            else 
                _context.SID = null;
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
