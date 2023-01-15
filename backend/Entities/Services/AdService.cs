using StackExchange.Redis;
using Neo4jClient;

public class AdService: IAdService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    private ICategoryService _category;
    
    public AdService(KviziramContext context,ICategoryService category, Utility utility) {
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

    public async Task<List<AdCategoryDto>?> ConnectAdCategoryAsync(Guid aduID, Guid cuID, int paid) {
        await ConnectAdCategoryQueryAsync(aduID, cuID, paid);
        IEnumerable<AccountPoco>? accounts = await _category.GetCategoryAccountsAsync(cuID);
        if (accounts != null) {
            Console.WriteLine(accounts.Count());
            List<Guid>? listGuids = accounts.Select(a => a.ID).ToList();
            await ConnectAdAccountAsync(aduID, listGuids);
        }
        List<Guid> listOneCuID = new();
        listOneCuID.Add(cuID);
        return await GetAdCategoryAsync(aduID, listOneCuID);
    }

    public async Task<string> RemoveAdCategoryAsync(Guid aduID, Guid cuID) {
        await RemoveAdCategoryQueryAsync(aduID, cuID);
        return ("Connection" + Msg.Deleted);
    }

    public async Task<List<AdCategoryDto>?> GetAdCategoryAsync(Guid aduID, List<Guid>? categoryGuids)  {
        return await GetAdCategoryQueryAsync(aduID, categoryGuids);
    }

    public async Task<List<AdAccountDto>?> ConnectAdAccountAsync(Guid aduID, List<Guid> accountGuids) {
        await ConnectAdAccountQueryAsync(aduID, accountGuids);
        return await GetAdAccountAsync(aduID, accountGuids);
    }

    public async Task<string> RemoveAdAccountAsync(Guid aduID, Guid auID) {
        await RemoveAdAccountQueryAsync(aduID, auID);
        return ("Connection" + Msg.Deleted);
    }

    public async Task<List<AdAccountDto>?> GetAdAccountAsync(Guid aduID, List<Guid>? accountGuids) {
        return await GetAdAccountQueryAsync(aduID, accountGuids);
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

    public async Task<string> SetMatchAdAccounts(Match match) {
        if (match.SetPlayerIDsScores != null && match.QuizID != null) {
            List<Guid> accountGuids = match.SetPlayerIDsScores.Keys.ToList();
            Guid categoryID = (await _neo.Cypher
                .Match("(q:Quiz)-[:IS_TYPE]->(c:Category)")
                .Where((Quiz q) => q.ID == match.QuizID)
                .Return(c => c.As<Category>().ID)
                .ResultsAsync).SingleOrDefault();
            List<Ad>? categoryAds = await _category.GetCategoryAdsAsync(categoryID);
            if (categoryAds != null)
                foreach(Ad ad in categoryAds)
                    await ConnectAdAccountAsync(ad.ID, accountGuids);
        return Msg.OperationDone;
        }
        return "";
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

    public async Task<List<AdCategoryDto>?> GetAdCategoryQueryAsync(Guid aduID, List<Guid>? categoryGuids) {
        if (categoryGuids == null)
            throw new KviziramException(Msg.Empty);
        var query = _neo.Cypher;
        if (categoryGuids.Count == 0)
            query = query.OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)").Where((Ad ad) => ad.ID == aduID);
        else
            query = query.Unwind(categoryGuids, "cuID").OptionalMatch("(ad:Ad)-[r:AD_CATEGORY]->(c:Category)").Where((Ad ad) => ad.ID == aduID).AndWhere("c.ID = cuID");
        
        var result = await query.Return((c,r) => new {
                // Ad = ad.As<Ad>(),
                Category = c.CollectAs<Category>(),
                AdCategory = r.CollectAs<AdCategoryDto>()
            }).ResultsAsync;
        var res = result.Single();
        if (res != null && res.AdCategory != null) {
            for(int i = 0; i < res.AdCategory.Count(); i++) {
                // res.AdCategory.ElementAt(i).Ad = res.Ad;
                res.AdCategory.ElementAt(i).Category = res.Category.ElementAt(i);
            }                
            return res.AdCategory.ToList();
        }
        return null;
    }

    public async Task ConnectAdAccountQueryAsync(Guid aduID, List<Guid> accountGuids) {
        var query = _neo.Cypher
            .Match("(advertisment:Ad)")
            .Where((Ad advertisment) => advertisment.ID == aduID)
            .With("advertisment AS ad")
            .Unwind(accountGuids, "auID")
            .Match("(a:Account)")
            .Where("a.ID = auID")
            .AndWhere("NOT (ad)-[:AD_ACCOUNT]->(a)")
            .Merge("(ad)-[r:AD_ACCOUNT { Blocked: false, Viewed: 0, Clicked: 0 }]->(a)");
        Console.WriteLine(query.Query.DebugQueryText);
        await query.ExecuteWithoutResultsAsync();
    }

    public async Task<List<AdAccountDto>?> GetAdAccountQueryAsync(Guid aduID, List<Guid>? accountGuids) {
        if (accountGuids == null)
            throw new KviziramException(Msg.Empty);
        var query = _neo.Cypher;
        if (accountGuids.Count == 0)
            query = query.OptionalMatch("(ad:Ad)-[r:AD_ACCOUNT]->(a:Account)").Where((Ad ad) => ad.ID == aduID);
        else
            query = query.Unwind(accountGuids, "auID").OptionalMatch("(ad:Ad)-[r:AD_ACCOUNT]->(a:Account)").Where((Ad ad) => ad.ID == aduID).AndWhere("a.ID = auID");
        
        var result = await query.Return((a,r) => new {
                // Ad = ad.As<Ad>(),
                AccountPoco = a.CollectAs<AccountPoco>(),
                AdAccount = r.CollectAs<AdAccountDto>()
            }).ResultsAsync;
        var res = result.Single();
        if (res != null && res.AdAccount != null) {
            for(int i = 0; i < res.AdAccount.Count(); i++) {
                // res.AdAccount.ElementAt(i).Ad = res.Ad;
                res.AdAccount.ElementAt(i).AccountPoco = res.AccountPoco.ElementAt(i);
            }                
            return res.AdAccount.ToList();
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