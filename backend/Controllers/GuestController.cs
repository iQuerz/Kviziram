using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private IGuestService _kviziram;

    public GuestController(IGuestService kviziram): base()
    {
        _kviziram = kviziram;
    }
}
