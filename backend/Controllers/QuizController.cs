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
    public async Task<ActionResult<Quiz>> GetQuiz(Guid uID) {
        return Ok(await _kviziram.GetQuizAsync(uID));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<QuizDto>>> SearchQuizzes([FromQuery] QuizQuery quizQuery) {
        return Ok(await _kviziram.SearchQuizzesAsync(quizQuery));
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Quiz>> CreateQuiz(Quiz newQuiz) {
        return Ok(await _kviziram.CreateQuizAsync(newQuiz));
    }
    #endregion

    #region PUT Methods
    #endregion

    #region DELETE Methods
    #endregion
}