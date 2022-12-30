using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class QuizController : ControllerBase
{
    private IQuizService _kviziram;

    public QuizController(IQuizService kviziram): base()
    {
        _kviziram = kviziram;
    }

    #region GET Methods
    [HttpGet]
    public async Task<ActionResult<Quiz>> GetQuiz(Guid quID) {
        return Ok(await _kviziram.GetQuizAsync(quID));
    }

    [HttpGet("{quID}/ratings")]
    public async Task<ActionResult<List<AccountQuizRatingDto>>> GetQuizRatings(Guid quID) {
        return Ok(await _kviziram.GetQuizRatingsAsync(quID));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<QuizDto>>> SearchQuizzes([FromQuery] QuizQuery quizQuery) {
        return Ok(await _kviziram.SearchQuizzesAsync(quizQuery));
    }

    [HttpGet("{quID}/achievement/set/{acuID}")]
    public async Task<ActionResult<string>> SetQuizAchievement(Guid quID, Guid acuID) {
        //Samo admin
        return Ok(await _kviziram.ConnectQuizAchievementAsync(quID, acuID));
    }

    [HttpGet("{quID}/achievement/remove/{acuID}")]
    public async Task<ActionResult<string>> RemoveQuizAchievement(Guid quID, Guid acuID) {
        //Samo admin
        return Ok(await _kviziram.DisconnectQuizAchievementAsync(quID, acuID));
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Quiz>> CreateQuiz(Quiz newQuiz) {
        return Ok(await _kviziram.CreateQuizAsync(newQuiz));
    }
    #endregion

    #region PUT Methods
    [HttpPut]
    public async Task<ActionResult<Quiz>> UpdateQuiz(Quiz updatedQuiz) {
        return Ok(await _kviziram.UpdateQuizAsync(updatedQuiz));
    }
    #endregion

    #region DELETE Methods
    [HttpDelete]
    public async Task<ActionResult<string>> DeleteQuizAsync(Guid quID) {
        return Ok(await _kviziram.DeleteQuizAsync(quID));
    }
    #endregion
}