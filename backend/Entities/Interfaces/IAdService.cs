public interface IAdService
{
    public Task<Ad?> GetAdAsync(Guid aduID);
    public Task<List<Ad>?> GetAllAdsAsync();
    public Task<Ad?> CreateAdAsync(Ad newAd);
    public Task<Ad?> UpdateAdAsync(Ad updatedAd);
    public Task<string> DeleteAdAsync(Guid aduID);  

    public Task<AdCategoryDto?> GetAdCategoryAsync(Guid aduID, Guid cuID);
    public Task<List<AdCategoryDto>?> GetAdCategoriesAsync(Guid aduID);
    public Task<AdCategoryDto?> ConnectAdCategoryAsync(Guid aduID, Guid cuID, int paid);  
    public Task<string> RemoveAdCategoryAsync(Guid aduID, Guid cuID);

    public Task<List<AdAccountDto>?> GetAdAccountAsync(Guid aduID, List<Guid> accountGuids);
    public Task<List<AdAccountDto>?> GetAdAccountsAsync(Guid aduID);
    public Task<List<AdAccountDto>?> ConnectAdAccountAsync(Guid aduID, List<Guid> accountGuids);  
    public Task<string> RemoveAdAccountAsync(Guid aduID, Guid auID);

    public Task<string> BlockAdAccountAsync(Guid aduID, Guid auID);
    public Task<string> IncrementViewedAdAccountAsync(Guid aduID, Guid auID);
    public Task<string> IncrementClickedAdAccountAsync(Guid aduID, Guid auID);
}
