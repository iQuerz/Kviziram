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
}