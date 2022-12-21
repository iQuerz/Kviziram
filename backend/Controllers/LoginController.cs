using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private ILoginRegisterService _kviziram;

    public LoginController(ILoginRegisterService kviziram): base() {
        _kviziram = kviziram;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login() {
        string? authHeader = HttpContext.Request.Headers["Authentication"];
        return Ok(await _kviziram.login(authHeader));
    }    
}
