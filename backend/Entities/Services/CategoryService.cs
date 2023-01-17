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
    public async Task<Category?> GetCategoryAsync(Guid uID) {
        Category? category = await GetCategoryQueryAsync(uID);
        if (category == null)
            throw new KviziramException(Msg.NoCategory);
        return category;
    }

    public async Task<List<Category>> GetAllCategoriesAsync() {
        return await GetAllCategoriesQueryAsync();
    }

    public async Task<List<AccountPoco>?> GetCategoryAccountsAsync(Guid cuID) {
        return await GetCategoryAccountsQueryAsync(cuID);
    }


    public async Task<Category> CreateCategoryAsync(Category newCategory) {
        newCategory.ID = Guid.NewGuid();
        await CreateCategoryQueryAsync(newCategory);
        return newCategory;
    }

    public async Task<Category> UpdateCategoryAsync(Category updatedCategory) {
        Category? categoryExists = await GetCategoryAsync(updatedCategory.ID);
        if (categoryExists == null)
            throw new KviziramException(Msg.NoCategory);
        await UpdateCategoryQueryAsync(updatedCategory);
        return updatedCategory;
    }

    public async Task<string> DeleteCategoryAsync(Guid uID) {
        Category? categoryExists = await GetCategoryAsync(uID);
        if (categoryExists == null)
            throw new KviziramException(Msg.NoCategory);
        await DeleteCategoryQueryAsync(uID);
        return ("Category: " + uID + Msg.Deleted);
    }

    public async Task<List<Ad>?> GetCategoryAdsAsync(Guid cuID) {
        return await GetCategoryAdsQueryAsync(cuID);
    }

    #endregion

    #region Helper Functions
    public async Task<Category?> GetCategoryQueryAsync(Guid uID) {
        var query = await _neo.Cypher
            .OptionalMatch("(c:Category)")
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

    public async Task<List<AccountPoco>?> GetCategoryAccountsQueryAsync(Guid cuID) {
        var query = await _neo.Cypher
            .OptionalMatch("(c:Category)<-[:PREFERS]-(a:Account)")
            .Where((Category c) => c.ID == cuID)
            .Return(a => a.As<AccountPoco>())
            .ResultsAsync;
        if (query.Count() == 1) return null; 
        else return query.ToList(); 
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
            .DetachDelete("c")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<List<Ad>?> GetCategoryAdsQueryAsync(Guid cuID) {
        var query = _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)")
            .Where((Category c) => c.ID == cuID)
            .Return( ad => ad.As<Ad>());
        var result = await query.ResultsAsync;
        if (result.Count() != 0) {
            return result.ToList();
        }
        return null;
    }
    #endregion

}