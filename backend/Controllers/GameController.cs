using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private IGameService _kviziram;

    public GameController(IGameService kviziram): base()
    {
        _kviziram = kviziram;
    }

    #region GET Methods
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Match>> CreateGame(Match game) {
        return Ok(await _kviziram.CreateGameAsync(game));
    }
    #endregion

    #region PUT Methods
    #endregion

    #region DELETE Methods
    #endregion
}