using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AdController : ControllerBase
{
    private IAdService _kviziram;

    public AdController(IAdService kviziram): base()
    {
        _kviziram = kviziram;
    }
}