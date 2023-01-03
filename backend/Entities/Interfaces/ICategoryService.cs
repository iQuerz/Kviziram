public interface ICategoryService
{
    public Task<Category?> GetCategoryAsync(Guid? uID);
    public Task<List<Category>> GetAllCategoriesAsync();
    public Task<Category> CreateCategoryAsync(Category newCategory);
    public Task<Category> UpdateCategoryAsync(Category updatedCategory);
    public Task<string> DeleteCategoryAsync(Guid uID);




}