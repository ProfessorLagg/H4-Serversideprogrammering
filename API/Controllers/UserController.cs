using API.Data;
using API.Data.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        private readonly H4serversideTodoContext DatabaseContext;

        public UserController(H4serversideTodoContext dbContext) {
            this.DatabaseContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() {
            IList<Account> accounts = await DatabaseContext.Accounts.ToListAsync();
            if (accounts is null) {
                throw new ArgumentNullException(nameof(accounts));
            }
            return Ok(accounts);
        }
    }
}
