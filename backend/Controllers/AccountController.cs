using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        IAccountService _kviziram;

        public AccountController(IAccountService kviziram, Utility utility): base() {
            _kviziram = kviziram;
            utility.CallerExists();
        }
        
        #region GET Methods
        [HttpGet]
        public async Task<ActionResult<AccountPoco>> GetAccountView(Guid uID) {
            return Ok(await _kviziram.GetAccountAsync(uID));
        }

        [HttpGet("me")]
        public async Task<ActionResult<AccountPoco>> GetMyAccount() {
            return Ok(await _kviziram.GetMyAccount());
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<AccountPoco>>> GetAllAccounts() {
            return Ok(await _kviziram.GetAllAccountsAsync());
        }

        [HttpGet("search/name={username}")]
        public async Task<ActionResult<List<AccountPoco>>> SearchAccountByName(string username) {
            return Ok(await _kviziram.GetAccountsByUsername(username));
        }

        [HttpGet("me/friends/all/{rState}")]
        public async Task<ActionResult<List<AccountPoco>>> GetFriends(RelationshipState rState) {
            return Ok(await _kviziram.GetFriendsAsync(rState));
        }

        [HttpGet("me/friend/{fuID}/request")]
        public async Task<ActionResult> RequestRelationship(Guid fuID) {
            return Ok(await _kviziram.RequestRelationshipAsync(fuID));
        }

        [HttpGet("me/friend/{fuID}/request/{answer}")]
        public async Task<ActionResult> AnswerRelationship(Guid fuID, RelationshipState answer) {
            return Ok(await _kviziram.AnswerRelationshipAsync(fuID, answer));
        }

        [HttpGet("me/friend/{fuID}/block")]
        public async Task<ActionResult> BlockRelationship(Guid fuID) {
            return Ok(await _kviziram.BlockRelationshipAsync(fuID));
        }

        [HttpGet("me/quiz/{quID}/rating")]
        public async Task<ActionResult<QuizRatingDto>> GetRating(Guid quID) {
            return Ok(await _kviziram.GetRatingAsync(quID));
        }

        [HttpGet("me/categories/preferred/get")]
        public async Task<ActionResult<List<Category>>> GetPreferredCategories() {
            return Ok(await _kviziram.GetPreferredCategoriesAsync());
        }

        [HttpGet("{auID}/achievements/{acuID}/set")]
        public async Task<ActionResult<string>> SetUpdateAchievements(Guid auID, Guid acuID) {
            return Ok(await _kviziram.SetUpdateAchievementAsync(auID, acuID));
        }

        [HttpGet("{auID}/achievements/get")]
        public async Task<ActionResult<List<Achievement>>> GetAccountAchievements(Guid auID) {
            return Ok(await _kviziram.GetAccountAchievementsAsync(auID));
        }

        [HttpGet("me/ads/recommended")]
        public async Task<ActionResult<Ad>> GetRecommendedAds() {
            return Ok(await _kviziram.GetRecommendedAdsAsync());
        }

        [HttpGet("{auID}/friends/recommended")]
        public async Task<ActionResult<List<AccountPoco>>> GetRecommendedFriends(Guid auID) {
            return Ok(await _kviziram.GetRecommendedFriendsAsync(auID));
        }

        [HttpGet("{auID}/match/players")]
        public async Task<ActionResult<List<AccountPoco>>> GetMatchPlayers(Guid auID) {
            return Ok(await _kviziram.RecommendedPlayersFromMatchAsync(auID));
        }

        [HttpGet("{auID}/fof/recommended")]
        public async Task<ActionResult<List<AccountPoco>>> GetFof(Guid auID) {
            return Ok(await _kviziram.GetFriendsOfFriendsAsync(auID));
        }

        [HttpGet("me/quiz/recommended")]
        public async Task<ActionResult<List<Quiz>>> GetRecommendedQuizzes() {
            return Ok(await _kviziram.GetRecommendedQuizzesAsync());
        }
        #endregion
        
        #region POST Methods
        [HttpPost("me/quiz/{quID}/rating/add")]
        public async Task<ActionResult<bool>> AddRating(Guid quID, [FromBody] QuizRatingDto newRating) {
            return Ok(await _kviziram.AddRatingAsync(quID, newRating));
        }

        [HttpPost("me/categories/preferred/set")]
        public async Task<ActionResult<string>> SetPreferredCategories(List<Guid> categoryGuids) {
            return Ok(await _kviziram.SetPreferredCategoryAsync(categoryGuids));
        }
        #endregion

        #region PUT Methods
        [HttpPut("me/quiz/{quID}/rating/update")]
        public async Task<ActionResult<bool>> UpdateRating(Guid quID, [FromBody] QuizRatingDto updatedRating) {
            return Ok(await _kviziram.UpdateRatingAsync(quID, updatedRating));
        }
        #endregion

        #region DELETE Methods
        [HttpDelete("me/quiz/{quID}/rating/remove")]
        public async Task<ActionResult<bool>> RemoveRating(Guid quID) {
            return Ok(await _kviziram.RemoveRatingAsync(quID));
        }

        [HttpDelete("me/friend/{fuID}/remove")]
        public async Task<ActionResult> RemoveRelationship(Guid fuID) {            
            return Ok(await _kviziram.RemoveRelationshipAsync(fuID));
        }

        [HttpDelete("me/categories/preferred/{cuID}/remove")]
        public async Task<ActionResult<string>> RemovePreferredCategory(Guid cuID) {
            return Ok(await _kviziram.RemovePreferredCategoryAsync(cuID));
        }
        #endregion
        
    }
}