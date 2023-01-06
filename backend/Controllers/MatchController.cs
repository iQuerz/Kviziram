using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MatchController : ControllerBase
{
    private IMatchService _kviziram;

    public MatchController(IMatchService kviziram): base()
    {
        _kviziram = kviziram;
    }

    #region GET Methods
    #endregion

    #region POST Methods
    [HttpPost("save")]
    public async Task<ActionResult<string>> SaveMatch(Match match) {
        return Ok(await _kviziram.SaveMatchAsync(match));
    }
    #endregion

    #region PUT Methods
    #endregion

    #region DELETE Methods
    #endregion
}