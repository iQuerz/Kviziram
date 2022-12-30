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

        [HttpGet("me/quiz/{quID}/rating")]
        public async Task<ActionResult<QuizRatingDto>> GetRating(Guid quID) {
            return Ok(await _kviziram.GetRatingAsync(quID));
        }

        #endregion
        
        #region POST Methods
        [HttpPost("me/quiz/{quID}/rating/add")]
        public async Task<ActionResult<bool>> AddRating(Guid quID, [FromBody] QuizRatingDto newRating) {
            return Ok(await _kviziram.AddRatingAsync(quID, newRating));
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
        #endregion
        
    }
}