using API.Data;
using API.Data.Model;
using API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using static API.Services.H4AuthService;

namespace API.Controllers {
    [Route("[controller]")]
    [ApiController]
    public sealed class AccountController : ControllerBase {

        private readonly H4serversideTodoContext _dbContext;
        private readonly H4AuthService authService;


        public AccountController(H4serversideTodoContext dbContext, H4AuthService authService) {
            this._dbContext = dbContext;
            this.authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() {
            IList<Account> accounts = await _dbContext.Accounts.ToListAsync();
            if (accounts is null) {
                throw new ArgumentNullException(nameof(accounts));
            }
            return Ok(accounts);
        }

        public sealed class AuthenticateRequest : IDisposable {
            public string Login { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;

            public void Dispose() {
                this.Login = "";
                this.PasswordHash = "";
            }
        }
        public sealed class AuthenticateResponse {
            public int UserId { get; set; } = -1;
            public string Login { get; set; } = string.Empty;
            public bool Authenticated { get; set; } = false;
            public int? SessionId { get; set; } = null;
            public string? Message { get; set; } = string.Empty;
            public AuthenticateResponse() { }
            public void Load(AuthResult authResult) {
                if (authResult.Account is not null) {
                    this.UserId = authResult.Account.Id;
                    this.Login = authResult.Account.Login;
                }
                if (authResult.Session is not null) {
                    this.SessionId = authResult.Session.Id;
                }
            }

        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request) {

            if (request is null) { return BadRequest(); }
            if (string.IsNullOrWhiteSpace(request.Login)) { return BadRequest($"username cannot be null or empty"); }
            if (string.IsNullOrWhiteSpace(request.PasswordHash)) { return BadRequest($"passwordHash cannot be null or empty"); }

            AuthenticateResponse resp = new();
            resp.Login = request.Login;

            AuthResult authResult = await authService.AuthenticateAsync(request.Login, request.PasswordHash);
            if (!authResult.Found) { return NotFound(request.Login); }
            if (!authResult.Authenticated) { return Unauthorized(); }




            throw new NotImplementedException();
        }
    }
}
