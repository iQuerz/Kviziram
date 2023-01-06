using StackExchange.Redis;
using Neo4jClient;

public class AdService: IAdService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    private ICategoryService _category;
    
    public AdService(KviziramContext context, ICategoryService category, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _category = category;
    }

    #region Main Functions
    public async Task<Ad?> GetAdAsync(Guid aduID) {
        return await GetAdQueryAsync(aduID);
    }

    public async Task<List<Ad>?> GetAllAdsAsync() {
        return await GetAllAdsQueryAsync();
    }

    public async Task<Ad?> CreateAdAsync(Ad newAd) {
        newAd.ID = Guid.NewGuid();
        return await CreateAdQueryAsync(newAd);
    }

    public async Task<Ad?> UpdateAdAsync(Ad updatedAd) {
        Ad? AdExists = await GetAdAsync(updatedAd.ID);
        if (AdExists == null)
            throw new KviziramException(Msg.NoAd);
        return await UpdateAdQueryAsync(updatedAd);
    }

    public async Task<string> DeleteAdAsync(Guid aduID) {
        Ad? AdExists = await GetAdAsync(aduID);
        if (AdExists == null)
            throw new KviziramException(Msg.NoAd);
        await DeleteAdQueryAsync(aduID);
        return ("Ad: " + aduID + Msg.Deleted);
    }

    public async Task<AdCategoryDto?> ConnectAdCategoryAsync(Guid aduID, Guid cuID, int paid) {
        await ConnectAdCategoryQueryAsync(aduID, cuID, paid);
        return await GetAdCategoryAsync(aduID, cuID);
    }

    public async Task<string> RemoveAdCategoryAsync(Guid aduID, Guid cuID) {
        await RemoveAdCategoryQueryAsync(aduID, cuID);
        return ("Connection" + Msg.Deleted);
    }

    public async Task<AdCategoryDto?> GetAdCategoryAsync(Guid aduID, Guid cuID) {
        return await GetAdCategoryQueryAsync(aduID, cuID);
    }

    public async Task<List<AdCategoryDto>?> GetAdCategoriesAsync(Guid aduID) {
        return await GetAdCategoriesQueryAsync(aduID);
    }

    public async Task<List<AdAccountDto>?> ConnectAdAccountAsync(Guid aduID, List<Guid> accountGuids) {
        await ConnectAdAccountQueryAsync(aduID, accountGuids);
        return await GetAdAccountAsync(aduID, accountGuids);
    }

    public async Task<string> RemoveAdAccountAsync(Guid aduID, Guid auID) {
        await RemoveAdAccountQueryAsync(aduID, auID);
        return ("Connection" + Msg.Deleted);
    }

    public async Task<List<AdAccountDto>?> GetAdAccountAsync(Guid aduID, List<Guid> accountGuids) {
        return await GetAdAccountQueryAsync(aduID, accountGuids);
    }

    public async Task<List<AdAccountDto>?> GetAdAccountsAsync(Guid aduID) {
        return await GetAdAccountsQueryAsync(aduID);
    }

    public async Task<string> BlockAdAccountAsync(Guid aduID, Guid auID) {
        await BlockAdAccountQueryAsync(aduID, auID);
        return Msg.AdBlocked;
    }

    public async Task<string> IncrementViewedAdAccountAsync(Guid aduID, Guid auID) {
        await IncrementViewedAdAccountQueryAsync(aduID, auID);
        return Msg.OperationDone;
    }

    public async Task<string> IncrementClickedAdAccountAsync(Guid aduID, Guid auID) {
        await IncrementClickedAdAccountQueryAsync(aduID, auID);
        return Msg.OperationDone;
    }
    #endregion

    #region Helper Functions
    public async Task<Ad?> GetAdQueryAsync(Guid aduID) {
        var query = await _neo.Cypher
            .OptionalMatch("(ad:Ad)")
            .Where((Ad ad) => ad.ID == aduID)
            .Return(ad => ad.As<Ad>())
            .ResultsAsync;
        return query.SingleOrDefault();
    }

    public async Task<List<Ad>?> GetAllAdsQueryAsync() {
        var query = await _neo.Cypher
            .OptionalMatch("(ad:Ad)")
            .Return(ad => ad.As<Ad>())
            .ResultsAsync;
        return query.ToList();
    }

    public async Task<Ad?> CreateAdQueryAsync(Ad newAd) {
        await _neo.Cypher
            .Create("(ad:Ad $prop)")
            .WithParam("prop", newAd)
            .ExecuteWithoutResultsAsync();
        return await GetAdAsync(newAd.ID);
    }

    public async Task<Ad?> UpdateAdQueryAsync(Ad updatedAd) {
        await _neo.Cypher
            .OptionalMatch("(ad:Ad)")
            .Where((Ad ad) => ad.ID == updatedAd.ID)
            .Set("ad = $prop")
            .WithParam("prop", updatedAd)
            .ExecuteWithoutResultsAsync();
        return await GetAdAsync(updatedAd.ID);
    }

    public async Task DeleteAdQueryAsync(Guid aduID) {
        await _neo.Cypher
            .Match("(ad:Ad)")
            .Where((Ad ad) => ad.ID == aduID)
            .DetachDelete("ad")
            .ExecuteWithoutResultsAsync();
    }

    public async Task ConnectAdCategoryQueryAsync(Guid aduID, Guid cuID, int paid) {
        await _neo.Cypher
            .Match("(ad:Ad)")
            .Where((Ad ad) => ad.ID == aduID)
            .Match("(c:Category)")
            .Where((Category c) => c.ID == cuID)
            .Merge("(ad)-[r:AD_CATEGORY]->(c)")
            .Set("r.Paid = $prop")
            .WithParam("prop", paid)
            .ExecuteWithoutResultsAsync();
    }

    public async Task RemoveAdCategoryQueryAsync(Guid aduID, Guid cuID) {
        await _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Category c) => c.ID == cuID)
            .Delete("r")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task<List<AdCategoryDto>?> GetAdCategoriesQueryAsync(Guid aduID) {
        var query = _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)")
            .Where((Ad ad) => ad.ID == aduID)
            .Return((c,r) => new {
                Categories = c.CollectAs<Category>(),
                CategoryPaid = r.CollectAs<AdCategoryDto>()
            });
        var result = (await query.ResultsAsync).Single();
        if (result.Categories.Count() != 0) {
            for(int i = 0; i < result.Categories.Count(); i++) {
                result.CategoryPaid.ElementAt(i).Ad = null;
                result.CategoryPaid.ElementAt(i).Category = result.Categories.ElementAt(i);
            }
            return result.CategoryPaid.ToList();
        }
        return null;
    }

    public async Task<AdCategoryDto?> GetAdCategoryQueryAsync(Guid aduID, Guid cuID) {
        var query = _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Category c) => c.ID == cuID)
            .Return((ad,c,r) => new {
                Ad = ad.As<Ad>(),
                Category = c.As<Category>(),
                AdCategory = r.As<AdCategoryDto>()
            });
        var result = (await query.ResultsAsync).SingleOrDefault();
        if (result != null) {
            result.AdCategory.Ad = result.Ad;
            result.AdCategory.Category = result.Category;
            return result.AdCategory;
        }
        return null;
    }

    public async Task ConnectAdAccountQueryAsync(Guid aduID, List<Guid> accountGuids) {
        await _neo.Cypher
            .Match("(adertisment:Ad)")
            .Where((Ad advertisment) => advertisment.ID == aduID)
            .With("advertisment AS ad")
            .Unwind(accountGuids, "auID")
            .Match("(a:Account)")
            .Where("a.ID == auID")
            .AndWhere("NOT (ad)-[:AD_ACCOUNT]->(a)")
            .Merge("(ad)-[r:AD_ACCOUNT { Blocked: false, Viewed: 0, Clicked: 0 }]->(a)")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<List<AdAccountDto>?> GetAdAccountQueryAsync(Guid aduID, List<Guid> accountGuids) {
        var query = _neo.Cypher
            .Unwind(accountGuids, "auID")
            .OptionalMatch("(ad:Ad)-[r:AD_ACCOUNT]->(a:Account)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere("a.ID == auID")
            .Return((ad,a,r) => new {
                Ad = ad.As<Ad>(),
                AccountPoco = a.CollectAs<AccountPoco>(),
                AdAccount = r.CollectAs<AdAccountDto>()
            });
        var result = (await query.ResultsAsync).FirstOrDefault();
        if (result != null && result.AdAccount != null) {
            for(int i = 0; i < result.AdAccount.Count(); i++) {
                result.AdAccount.ElementAt(i).Ad = result.Ad;
                result.AdAccount.ElementAt(i).AccountPoco = result.AccountPoco.ElementAt(i);
            }                
            return result.AdAccount.ToList();
        }
        return null;
    }

    public async Task RemoveAdAccountQueryAsync(Guid aduID, Guid auID) {
        await _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_ACCOUNT]->(a:Account)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Account a) => a.ID == auID)
            .Delete("r")
            .ExecuteWithoutResultsAsync();
    }
    
    public async Task<List<AdAccountDto>?> GetAdAccountsQueryAsync(Guid aduID) {
        var query = _neo.Cypher
            .OptionalMatch("(ad:Ad)-[r:AD_ACCOUNT]->(a:Account)")
            .Where((Ad ad) => ad.ID == aduID)
            .Return((a,r) => new {
                Accounts = a.CollectAs<AccountPoco>(),
                AdAccounts = r.CollectAs<AdAccountDto>()
            });
        var result = (await query.ResultsAsync).Single();
        if (result.Accounts.Count() != 0) {
            for(int i = 0; i < result.Accounts.Count(); i++) {
                result.AdAccounts.ElementAt(i).Ad = null;
                result.AdAccounts.ElementAt(i).AccountPoco = result.Accounts.ElementAt(i);
            }
            return result.AdAccounts.ToList();
        }
        return null;
    }

    public async Task BlockAdAccountQueryAsync(Guid aduID, Guid auID) {
        await _neo.Cypher
            .OptionalMatch("(ad)-[r:AD_ACCOUNT]->(a)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Account a) => a.ID == auID)
            .Set("r.Blocked = true")
            .ExecuteWithoutResultsAsync();
    }

    public async Task IncrementViewedAdAccountQueryAsync(Guid aduID, Guid auID) {
        await _neo.Cypher
            .OptionalMatch("(ad)-[r:AD_ACCOUNT]->(a)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Account a) => a.ID == auID)
            .Set("r.Viewed = r.Viewed + 1")
            .ExecuteWithoutResultsAsync();
    }

    public async Task IncrementClickedAdAccountQueryAsync(Guid aduID, Guid auID) {
        await _neo.Cypher
            .OptionalMatch("(ad)-[r:AD_ACCOUNT]->(a)")
            .Where((Ad ad) => ad.ID == aduID)
            .AndWhere((Account a) => a.ID == auID)
            .Set("r.Clicked = r.Clicked + 1")
            .ExecuteWithoutResultsAsync();
    }

    #endregion
    
}