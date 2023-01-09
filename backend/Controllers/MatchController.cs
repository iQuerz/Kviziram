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
    [HttpGet]
    public async Task<ActionResult<Match>> GetMatch(Guid muID) {
        return Ok(await _kviziram.GetMatchAsync(muID));
    }
    #endregion

    #region POST Methods
    [HttpPost("save")]
    public async Task<ActionResult<string>> SaveMatch(Match match) {
        return Ok(await _kviziram.SaveMatchAsync(match));
    }

    [HttpPost("search/history/{skip}/{limit}/q")]
    public async Task<ActionResult<List<Match>>> SearchMatchesHistory([FromQuery] MatchQuery matchQuery, [FromBody] FromToDate? fromToDate = null, int skip = 0, int limit = 10) {
        return Ok(await _kviziram.SearchMatchesHistoryAsync(matchQuery, fromToDate, skip, limit));
    }
    #endregion

    #region PUT Methods
    #endregion

    #region DELETE Methods
    #endregion
}