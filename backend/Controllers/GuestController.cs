using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private IGuestService _kviziram;
    private IGraphClient _neo;

    public GuestController(IGuestService kviziram, IGraphClient neo): base()
    {
        _kviziram = kviziram;
        _neo = neo;
    }

    [HttpPost("test/{id}")]
    public async Task<ActionResult<string?>> test(int id) {
        if (id == 1)
            throw new KviziramException("Uhvatio sam kriziram exceptio");
        if (id == 2)
            throw new Exception();
        if (id == 3)
            throw new Exception("Jbm li ga");
        await _kviziram.addString("aeradi", "hehehehe");
        return Ok(await _kviziram.getString("aeradi"));
    }

    [Route("getTest")]
    [HttpGet]
    public async Task<ActionResult<string>> getTest() {
        await Task.Delay(3000);
        return Ok("tes1t");
    }
}
