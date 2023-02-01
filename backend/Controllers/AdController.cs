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
    public async Task<ActionResult<Ad?>> GetAd(Guid aduID) {
        return Ok(await _kviziram.GetAdAsync(aduID));
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<Ad>>> GetAllAds() {
        return Ok(await _kviziram.GetAllAdsAsync());
    }

    [HttpGet("{aduID}/me/block")]
    public async Task<ActionResult<string>> BlockAdAccount(Guid aduID) {
        return Ok(await _kviziram.BlockAdAccountAsync(aduID));
    }

    [HttpGet("{aduID}/me/viewed")]
    public async Task<ActionResult<string>> IncrementViewedAdAccount(Guid aduID) {
        return Ok(await _kviziram.IncrementViewedAdAccountAsync(aduID));
    }

    [HttpGet("{aduID}/me/clicked")]
    public async Task<ActionResult<string>> IncrementClickedAdAccount(Guid aduID) {
        return Ok(await _kviziram.IncrementClickedAdAccountAsync(aduID));
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

    [HttpPost("{aduID}/connect/accounts")]
    public async Task<ActionResult<AdAccountDto>> ConnectAdAccount(Guid aduID, List<Guid> auID) {
        return Ok(await _kviziram.ConnectAdAccountAsync(aduID, auID));
    }

    [HttpPost("{aduID}/account")]
    public async Task<ActionResult<AdAccountDto>> GetAdAccount(Guid aduID, List<Guid> auID) {
        return Ok(await _kviziram.GetAdAccountAsync(aduID, auID));
    }

    [HttpPost("{aduID}/category")]
    public async Task<ActionResult<AdCategoryDto>> GetAdCategory(Guid aduID, List<Guid> categoryGuids) {
        return Ok(await _kviziram.GetAdCategoryAsync(aduID, categoryGuids));
    }
    #endregion

    #region PUT Methods
    [HttpPut]
    public async Task<ActionResult<Ad>> UpdateAd(Ad updatedAd) {
        return Ok(await _kviziram.UpdateAdAsync(updatedAd));
    }
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