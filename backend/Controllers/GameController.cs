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
    public async Task<ActionResult<List<AccountPoco>>> GetGameLobby(string inviteCode) {
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

    [HttpGet("{inviteCode}/answered")]
    public async Task<ActionResult<List<string>>> GetGameCurrentPlayersAnswered(string inviteCode) {
        return Ok(await _kviziram.GetPlayersAnswered(inviteCode));
    }

    [HttpGet("{inviteCode}")]
    public async Task<ActionResult<Match>> GetGame(string inviteCode) {
        return Ok(await _kviziram.GetGameAsync(inviteCode));
    }

    [HttpGet("recent/account/{playerGuid}")]
    public async Task<ActionResult<List<GameDto>>> GetLastPlayedGames(Guid playerGuid) {
        return Ok(await _kviziram.GetLastPlayedGamesAsync(playerGuid));
    }

    [HttpGet("/invite/{inviteCode}/account/{auID}")]
    public async Task<ActionResult> SendInvite(Guid auID, string inviteCode) {
        await _kviziram.SendInviteAsync(auID, inviteCode);
        return Ok(Msg.Invited);
    }

    [HttpGet("invites/account/{auID}")]
    public async Task<ActionResult<List<GameInviteDto>>> GetAllInvites(Guid auID) {
        return Ok(await _kviziram.GetAllInvitesAsync(auID));
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

    [HttpPost("invite/clicked")]
    public async Task<ActionResult> ClickInvite(GameInviteDto invite) {
        await _kviziram.ClickInviteAsync(invite);
        return Ok(Msg.InviteOpened);
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
    // [HttpGet("createPubSub/{channelName}")]
    // public async Task CreatePubSub(string channelName) {
    //     await _kviziram.CreatePubSubAsync(channelName);
    // }

    // [HttpGet("testPubSub/{channelName}/{msg}")]
    // public async Task testPubSub(string channelName, string msg) {
    //     await _kviziram.TestPubSubAsync(channelName, msg);
    // }
    // [HttpPost("test/converter")]
    // public async Task<ActionResult<GameDto>> ConvertMatchToGameDto([FromBody] Match game) {
    //     return Ok(await _kviziram.ConvertMatchToGameDtoAsync(game));
    // }

    // [HttpPost("test/savetohistory")]
    // public async Task<ActionResult<Match>> SaveGameToHistory([FromBody] Match game) {
    //     return Ok(await _kviziram.SaveGameToHistoryAsync(game));
    // }

    // [HttpGet("test/{inviteCode}/{auID}/{sid}")]
    // public async Task AddToLobby(string inviteCode, Guid auID, string sid) {
    //     await _kviziram.AddToLobby(inviteCode, auID, sid);
    // }
    #endregion
}