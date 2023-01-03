using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AchievementController : ControllerBase
{
    private IAchievementService _kviziram;

    public AchievementController(IAchievementService kviziram): base()
    {
        _kviziram = kviziram;
    }

    #region GET Methods
    [HttpGet]
    public async Task<ActionResult<Achievement>> GetAchievement(Guid uID) {
        return Ok(await _kviziram.GetAchievementAsync(uID));
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Achievement>>> GetAllAchievements() {
        return Ok(await _kviziram.GetAllAchievementsAsync());
    }
    
    [HttpGet("{acuID}/scoreboard")]
    public async Task<ActionResult<List<AchievedDto>>> GetAchievementScoreboard(Guid acuID) {
        return Ok(await _kviziram.GetAchievementScoreboardAsync(acuID));
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Achievement>> CreateAchievement(Achievement newAchievement) {
        return Ok(await _kviziram.CreateAchievementAsync(newAchievement));
    }
    #endregion

    #region PUT Methods
    [HttpPut]
    public async Task<ActionResult<Achievement>> UpdateAchievement(Achievement updatedAchievement) {
        return Ok(await _kviziram.UpdateAchievementAsync(updatedAchievement));
    }
    #endregion

    #region DELETE Methods
    [HttpDelete("{uID}")]
    public async Task<ActionResult> DeleteAchievement(Guid uID) {
        return Ok(await _kviziram.DeleteAchievementAsync(uID));
    }
    #endregion
}