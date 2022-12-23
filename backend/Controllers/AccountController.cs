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

        [HttpGet("{uid}")]
        public async Task<ActionResult<AccountView>> GetAccountView(Guid uid) {
            return Ok(await _kviziram.GetAccountViewAsync(uid));
        }

        [HttpGet("me/friend/{fuid}/request")]
        public async Task<ActionResult<RelationshipState>> RequestRelationship(Guid fuid) {
            await _kviziram.RequestRelationshipAsync(fuid);
            return Ok(RelationshipState.Pending);
        }
        
    }
}