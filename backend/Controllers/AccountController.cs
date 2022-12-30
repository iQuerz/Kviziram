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

        [HttpGet("me/friend/{fuID}/remove")]
        public async Task<ActionResult> RemoveRelationship(Guid fuID) {            
            return Ok(await _kviziram.RemoveRelationshipAsync(fuID));
        }

        [HttpGet("me/quiz/{quID}/rating/remove")]
        public async Task<ActionResult<bool>> RemoveRating(Guid quID) {            
            return Ok(await _kviziram.RemoveRatingAsync(quID));
        }
        #endregion
        
        #region POST Methods
        [HttpPost("me/quiz/{quID}/rating/add")]
        public async Task<ActionResult<bool>> RateQuiz(Guid quID, [FromBody] QuizRatingDto newRating) {
            return Ok(await _kviziram.RateQuizAsync(quID, newRating));
        }
        #endregion

        #region PUT Methods
        #endregion

        #region DELETE Methods
        #endregion
        
    }
}