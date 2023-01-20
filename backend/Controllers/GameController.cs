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
    [HttpGet("{inviteCode}/start")]
    public async Task<ActionResult<string>> StartGame(string inviteCode) {
        return Ok(await _kviziram.StartGameAsync(inviteCode));
    }

    [HttpGet("{inviteCode}/information")]
    public async Task<ActionResult<GameDto>> GetGameInformation(string inviteCode) {
        return Ok(await _kviziram.GetGameInformationAsync(inviteCode));
    }

    [HttpGet("{inviteCode}/lobby")]
    public async Task<ActionResult<Dictionary<string, string>>> GetGameLobby(string inviteCode) {
        return Ok(await _kviziram.GetGameLobbyAsync(inviteCode));
    }

    [HttpGet("{inviteCode}/scores")]
    public async Task<ActionResult<Dictionary<string, int>>> GetGameScores(string inviteCode) {
        return Ok(await _kviziram.GetGameScoresAsync(inviteCode));
    }

    [HttpGet("{inviteCode}/chat/{start}/{stop}")]
    public async Task<ActionResult<List<string>>> GetGameChat(string inviteCode, int start, int stop) {
        return Ok(await _kviziram.GetGameChatAsync(inviteCode, start, stop));
    }

    [HttpGet("{inviteCode}/question/{quizID}")]
    public async Task<ActionResult<QuestionDto>> GetGameCurrentQuestion(string inviteCode, Guid quizID) {
        return Ok(await _kviziram.GetGameCurrentQuestionAsync(inviteCode, quizID));
    }

    [HttpGet("recent/account/{playerGuid}")]
    public async Task<ActionResult<QuestionDto>> GetLastPlayedGames(Guid playerGuid) {
        return Ok(await _kviziram.GetLastPlayedGamesAsync(playerGuid));
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Match>> CreateGame(Match game) {
        return Ok(await _kviziram.CreateGameAsync(game));
    }

    [HttpPost("public/{skip}/{limit}/{asc}")]
    public async Task<ActionResult<List<GameDto>>> GetPublicGames([FromBody] FromToDate fromToDate, int skip, int limit, bool asc) {
        return Ok(await _kviziram.GetPublicGamesAsync(fromToDate, skip, limit, asc));
    }
    #endregion

    #region PUT Methods
    #endregion

    #region DELETE Methods
    [HttpDelete("{inviteCode}")]
    public async Task<ActionResult<bool>> RemoveGameFromRedis(string inviteCode) {
        return Ok(await _kviziram.RemoveGameFromRedisAsync(inviteCode));
    }
    #endregion

    #region Test Methods
    [HttpPost("test/converter")]
    public async Task<ActionResult<GameDto>> ConvertMatchToGameDto([FromBody] Match game) {
        return Ok(await _kviziram.ConvertMatchToGameDtoAsync(game));
    }

    [HttpPost("test/savetohistory")]
    public async Task<ActionResult<Match>> SaveGameToHistory([FromBody] Match game) {
        return Ok(await _kviziram.SaveGameToHistoryAsync(game));
    }

    [HttpGet("test/{inviteCode}/{auID}/{sid}")]
    public async Task AddToLobby(string inviteCode, Guid auID, string sid) {
        await _kviziram.AddToLobby(inviteCode, auID, sid);
    }
    #endregion
}