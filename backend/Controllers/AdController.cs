using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AdController : ControllerBase
{
    private IAdService _kviziram;

    public AdController(IAdService kviziram): base()
    {
        _kviziram = kviziram;
    }

    #region GET Methods
    [HttpGet]
    public async Task<ActionResult<Ad?>> GetAd(Guid acuID) {
        return Ok(await _kviziram.GetAdAsync(acuID));
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Ad>>> GetAllAds() {
        return Ok(await _kviziram.GetAllAdsAsync());
    }

    [HttpGet("{aduID}/category/all")]
    public async Task<ActionResult<List<AdCategoryDto>>> GetAdCategories(Guid aduID) {
        return Ok(await _kviziram.GetAdCategoriesAsync(aduID));
    }
    
    [HttpGet("{aduID}/category/{cuID}")]
    public async Task<ActionResult<AdCategoryDto>> GetAdCategory(Guid aduID, Guid cuID) {
        return Ok(await _kviziram.GetAdCategoryAsync(aduID, cuID));
    }

    [HttpGet("{aduID}/account/{auID}")]
    public async Task<ActionResult<AdAccountDto>> GetAdAccount(Guid aduID, List<Guid> auID) {
        return Ok(await _kviziram.GetAdAccountAsync(aduID, auID));
    }

    [HttpGet("{aduID}/account/all")]
    public async Task<ActionResult<List<AdAccountDto>>> GetAdAccounts(Guid aduID) {
        return Ok(await _kviziram.GetAdAccountsAsync(aduID));
    }

    [HttpGet("{aduID}/connect/accounts")]
    public async Task<ActionResult<AdAccountDto>> ConnectAdAccount(Guid aduID, List<Guid> auID) {
        return Ok(await _kviziram.ConnectAdAccountAsync(aduID, auID));
    }

    [HttpGet("{aduID}/account/{auID}/block")]
    public async Task<ActionResult<string>> BlockAdAccount(Guid aduID, Guid auID) {
        return Ok(await _kviziram.BlockAdAccountAsync(aduID, auID));
    }

    [HttpGet("{aduID}/account/{auID}/viewed")]
    public async Task<ActionResult<string>> IncrementViewedAdAccount(Guid aduID, Guid auID) {
        return Ok(await _kviziram.IncrementViewedAdAccountAsync(aduID, auID));
    }

    [HttpGet("{aduID}/account/{auID}/clicked")]
    public async Task<ActionResult<string>> IncrementClickedAdAccount(Guid aduID, Guid auID) {
        return Ok(await _kviziram.IncrementClickedAdAccountAsync(aduID, auID));
    }
    #endregion

    #region POST Methods
    [HttpPost]
    public async Task<ActionResult<Ad>> CreateAd(Ad newAd) {
        return Ok(await _kviziram.CreateAdAsync(newAd));
    }

    [HttpPost("{aduID}/connect/category/{cuID}")]
    public async Task<ActionResult<AdCategoryDto>> ConnectAdCategory(Guid aduID, Guid cuID, [FromBody] int paid) {
        return Ok(await _kviziram.ConnectAdCategoryAsync(aduID, cuID, paid));
    }
    #endregion

    #region PUT Methods
    [HttpPut]
    public async Task<ActionResult<Ad>> UpdateAd(Ad updatedAd) {
        return Ok(await _kviziram.UpdateAdAsync(updatedAd));
    }

    // [HttpPut]
    // public async Task<ActionResult<string>> UpdateAdAccount(List<Guid> guidList) {
       
    // }
    #endregion

    #region DELETE Methods
    [HttpDelete("{acuID}")]
    public async Task<ActionResult> DeleteAd(Guid acuID) {
        return Ok(await _kviziram.DeleteAdAsync(acuID));
    }

    [HttpDelete("{aduID}/remove/category/{cuID}")]
    public async Task<ActionResult<string>> RemoveAdCategory(Guid aduID, Guid cuID) {
        return Ok(await _kviziram.RemoveAdCategoryAsync(aduID, cuID));
    }

    
    [HttpDelete("{aduID}/remove/account/{auID}")]
    public async Task<ActionResult<string>> RemoveAdAccount(Guid aduID, Guid auID) {
        return Ok(await _kviziram.RemoveAdAccountAsync(aduID, auID));
    }
    #endregion
}