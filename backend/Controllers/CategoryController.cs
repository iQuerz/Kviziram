using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private ICategoryService _kviziram;

    public CategoryController(ICategoryService kviziram): base()
    {
        _kviziram = kviziram;
    }
}