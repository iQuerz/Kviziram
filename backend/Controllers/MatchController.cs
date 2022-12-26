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
}