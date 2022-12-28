using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private ICategoryService _kviziram;

    public CategoryController(ICategoryService kviziram, Utility utility): base()
    {
        _kviziram = kviziram;
        utility.IsCallerAccountAdmin();       
    }

    #region GET Methods
    [HttpGet]
    public async Task<ActionResult<Category>> GetCategory(Guid uID) {
        return Ok(await _kviziram.GetCategoryAsync(uID));
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Category>>> GetAllCategories() {
        return Ok(await _kviziram.GetAllCategoriesAsync());
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category newCategory) {
        return Ok(await _kviziram.CreateCategoryAsync(newCategory));
    }
    #endregion

    #region PUT Methods
    [HttpPut]
    public async Task<ActionResult<Category>> UpdateCategory(Category updatedCategory) {
        return Ok(await _kviziram.UpdateCategoryAsync(updatedCategory));
    }
    #endregion

    #region DELETE Methods
    [HttpDelete("{uID}")]
    public async Task<ActionResult> DeleteCategory(Guid uID) {
        return Ok(await _kviziram.DeleteCategoryAsync(uID));
    }
    #endregion
}