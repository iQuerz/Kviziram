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
}