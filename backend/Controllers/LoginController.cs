using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private ILoginRegisterService _kviziram;

    public LoginController(ILoginRegisterService kviziram): base() {
        _kviziram = kviziram;
    }

    [HttpGet]
    public async Task<ActionResult<string>> Login() {
        string? authHeader = HttpContext.Request.Headers["Authentication"];
        return Ok(await _kviziram.Login(authHeader));
    }

    [HttpGet("guest")]
    public async Task<ActionResult<string>> LoginGuest(string username) {
        return Ok(await _kviziram.LoginGuest(username));
    }

}
