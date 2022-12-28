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
            return Ok(await _kviziram.GetAccountViewAsync(uID));
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
        #endregion
        
        #region POST Methods
        #endregion

        #region PUT Methods
        #endregion

        #region DELETE Methods
        #endregion
        
    }
}