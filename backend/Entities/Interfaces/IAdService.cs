public interface IAdService
{
    public Task<Ad?> GetAdAsync(Guid aduID);
    public Task<List<Ad>?> GetAllAdsAsync();
    public Task<Ad?> CreateAdAsync(Ad newAd);
    public Task<Ad?> UpdateAdAsync(Ad updatedAd);
    public Task<string> DeleteAdAsync(Guid aduID);  

    public Task<List<AdCategoryDto>?> GetAdCategoryAsync(Guid aduID, List<Guid>? categoryGuids);
    public Task<List<AdCategoryDto>?> ConnectAdCategoryAsync(Guid aduID, Guid cuID, int paid);  
    public Task<string> RemoveAdCategoryAsync(Guid aduID, Guid cuID);

    public Task<List<AdAccountDto>?> GetAdAccountAsync(Guid aduID, List<Guid>? accountGuids);
    public Task<List<AdAccountDto>?> ConnectAdAccountAsync(Guid aduID, List<Guid> accountGuids);  
    public Task<string> RemoveAdAccountAsync(Guid aduID, Guid auID);

    public Task<string> BlockAdAccountAsync(Guid aduID);
    public Task<string> IncrementViewedAdAccountAsync(Guid aduID);
    public Task<string> IncrementClickedAdAccountAsync(Guid aduID);

    public Task<string> SetMatchAdAccounts(Match match);
}
