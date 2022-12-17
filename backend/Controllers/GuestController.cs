using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private IGuestService _kviziram;

    public GuestController(IGuestService kviziram): base()
    {
        _kviziram = kviziram;
    }

    [HttpPost("test/{id}")]
    public async Task<ActionResult<string?>> test(int id) {
        await _kviziram.addString("aeradi", "hehehehe");
        return Ok(await _kviziram.getString("aeradi"));
    }

    [Route("getTest")]
    [HttpGet]
    public async Task<ActionResult<string>> getTest() {
        await Task.Delay(3000);
        return Ok("string");
    }
}
