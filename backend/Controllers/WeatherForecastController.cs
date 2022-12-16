using Microsoft.AspNetCore.Mvc;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private KviziramService _kviziram;

    public WeatherForecastController(KviziramService kviziram): base()
    {
        _kviziram = kviziram;
    }

    [Route("test/{id}")]
    [HttpPost]
    public async Task<ActionResult<string?>> test(int id) {
        await _kviziram.addString("aeradi", "hehehehe");
        return Ok(await _kviziram.getString("aeradi"));
    }
}
