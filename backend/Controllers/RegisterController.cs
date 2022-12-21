using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    private ILoginRegisterService _kviziram;

    public RegisterController(ILoginRegisterService kviziram): base() {
        _kviziram = kviziram;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> register([FromBody] Account newAccount) {        
        return Ok(await _kviziram.register(newAccount));
    } 
}