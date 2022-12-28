using StackExchange.Redis;
using Neo4jClient;

public class CategoryService: ICategoryService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public CategoryService(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    public async Task<Category> GetCategoryAsync(Guid uID) {
        return await GetCategoryQueryAsync(uID);
    }

    public async Task<List<Category>> GetAllCategoriesAsync() {
        return await GetAllCategoriesQueryAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category newCategory) {
        newCategory.ID = Guid.NewGuid();
        await CreateCategoryQueryAsync(newCategory);
        return newCategory;
    }

    public async Task<Category> UpdateCategoryAsync(Category updatedCategory) {
        Category categoryExists = await GetCategoryAsync(updatedCategory.ID);
        if (categoryExists == null)
            throw new KviziramException(Msg.NoCategory);
        await UpdateCategoryQueryAsync(updatedCategory);
        return updatedCategory;
    }

    public async Task<string> DeleteCategoryAsync(Guid uID) {
        Category categoryExists = await GetCategoryAsync(uID);
        if (categoryExists == null)
            throw new KviziramException(Msg.NoCategory);
        await DeleteCategoryQueryAsync(uID);
        return ("Category: " + uID + Msg.Deleted);
    }
    #endregion

    #region Helper Functions
    public async Task<Category> GetCategoryQueryAsync(Guid uID) {
        var query = await _neo.Cypher
            .Match("(c:Category)")
            .Where((Category c) => c.ID == uID)
            .Return(c => c.As<Category>())
            .ResultsAsync;
        return query.Single();
    }

    public async Task<List<Category>> GetAllCategoriesQueryAsync() {
        var query = await _neo.Cypher
            .Match("(c:Category)")
            .Return(c => c.As<Category>())
            .ResultsAsync;
        return query.ToList();
    }

    public async Task CreateCategoryQueryAsync(Category newCategory) {
        await _neo.Cypher
            .Create("(c:Category $prop)")
            .WithParam("prop", newCategory)
            .ExecuteWithoutResultsAsync();
    }

    public async Task UpdateCategoryQueryAsync(Category updatedCategory) {
        await _neo.Cypher
            .Match("(c:Category)")
            .Where((Category c) => c.ID == updatedCategory.ID)
            .Set("c.Name = $name")
            .WithParam("name", updatedCategory.Name)
            .ExecuteWithoutResultsAsync();
    }

    public async Task DeleteCategoryQueryAsync(Guid uID) {
        await _neo.Cypher
            .Match("(c:Category)")
            .Where((Category c) => c.ID == uID)
            .OptionalMatch("(c)<-[:QUIZ_CATEGORY]-(q:Quiz)")
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }
    #endregion

}