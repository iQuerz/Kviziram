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
            utility.SetAccountAndGuestCaller();
        }

        [HttpGet("{uid}")]
        public async Task<ActionResult<AccountView>> GetAccountView(Guid uid) {
            return Ok(await _kviziram.GetAccountViewAsync(uid));
        }
        
    }
}