using API.Data;
using API.Data.Model;
using API.Services;
using API.Utils.HashingUtils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;

using static API.Services.H4AuthService;

namespace API.Controllers {
    [Route("[controller]")]
    [ApiController]
    public sealed class AccountController : ControllerBase {

        private readonly H4serversideTodoContext _dbContext;
        private readonly H4AuthService authService;
        private readonly IHashingService _hashingService;

        public AccountController(H4serversideTodoContext dbContext, IHashingService hashingService) {
            this._dbContext = dbContext;
            this.authService = new(this._dbContext);
            this._hashingService = hashingService;
        }

        public sealed record class AuthenticateRequest(string Login, string Password);
        public sealed class AuthenticateResponse {
            public int AccountId { get; set; } = -1;
            public bool Authenticated { get; set; } = false;
            public Guid SessionToken { get; set; } = Guid.Empty;
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request) {
            AuthenticateResponse resp = new();

            if (request is null) {
                resp.Message = "request cannot be null";
                return BadRequest(resp);
            }
            if (string.IsNullOrWhiteSpace(request.Login)) {
                resp.Message = "username cannot be null or empty";
                return BadRequest(resp);
            }
            if (string.IsNullOrWhiteSpace(request.Password)) {
                resp.Message = "passwordHash cannot be null or empty";
                return BadRequest(resp);
            }

            AuthResult authResult = await authService.AuthenticateAsync(request.Login, request.Password);
            if (!authResult.Found || authResult.Account is null || !authResult.Authenticated) {
                resp.Message = "Incorrect Username or Password";
                return NotFound(resp); // Might be a bad idea
            }

            if (authResult.Session is null) {
                resp.Message = "Could not create session, please try again";
                return StatusCode(500, resp);
            }

            resp.AccountId = authResult.Account.Id;
            resp.Authenticated = true;
            resp.SessionToken = authResult.Session.Token;
            resp.Message = "Login success!";
            return Ok(resp);
        }


        public sealed record class CreateUserRequest(string Login, string Password);
        public sealed record class CreateUserResponse(int Id, string Login);
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request) {
            if (request is null) { return BadRequest(); }
            if (string.IsNullOrEmpty(request.Login)) { return BadRequest($"username cannot be null or empty"); }
            if (string.IsNullOrEmpty(request.Password)) { return BadRequest($"password cannot be null or empty"); }

            string login = request.Login.Trim().ToLower();
            bool loginExists = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Login == login) is not null;
            if (loginExists) { return Conflict($"Login {request.Login} already exists!"); }

            // TODO Check password Requirements

            Account account = new();
            account.Login = request.Login;

            // TODO Check password Requirements
            byte[] passwordHashBytes = await _hashingService.GetHashAsync(request.Password);
            account.PasswordHash = Convert.ToHexString(passwordHashBytes).ToLower();

            account.Cpr = null;
            var addResult = await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();


            Account outAccount = addResult.Entity;

            CreateUserResponse response = new(outAccount.Id, outAccount.Login);
            return Ok(response);
        }


        private record struct ValidateCprStringResult(bool Valid, string Message);
        /// <summary> The characters allowed in a cpr string, in sorted order </summary>
        private const string CprValidChars = @"-0123456789";
        private ValidateCprStringResult ValidateCprString(string cpr) {
            if (string.IsNullOrWhiteSpace(cpr)) { return new(false, "CPR string was null, empty or whitespace"); }

            ReadOnlySpan<char> validChars = CprValidChars;
            ReadOnlySpan<char> cprSpan = cpr;
            for (int i = 0; i < cprSpan.Length; i++) {
                char c = cprSpan[i];
                bool c_valid = validChars.BinarySearch(c) >= 0;
                if (!c_valid) { return new(false, "CPR contained invalid chars"); }
            }

            cpr = cpr.Trim().Replace("-", "");
            if (cpr.Length != 10) { return new(false, "CPR must be 10 chars, but was " + cpr.Length); }


            int dayint = int.Parse(cpr.Substring(0, 2));
            int monthint = int.Parse(cpr.Substring(2, 2));
            int yearint = int.Parse(cpr.Substring(4, 2));

            // https://da.wikipedia.org/wiki/CPR-nummer#Under_eller_over_100_%C3%A5r
            switch (cpr[7]) {
                case '0': yearint += 1900; break;
                case '1': goto case '0';
                case '2': goto case '0';
                case '3': goto case '0';
                case '4': yearint += yearint <= 36 ? 2000 : 1900; break;
                case '5': yearint += yearint <= 57 ? 2000 : 1800; break;
                case '6': goto case '5';
                case '7': goto case '5';
                case '8': goto case '5';
                case '9': goto case '4';
                default: throw new UnreachableException($"should not be possible for '{cpr[7]}' to be present in CPR");
            }

            if (yearint < 1835) {
                return new(false, $"year part of CPR '{yearint}' could never match an actual CPR number");
            }
            if (monthint < 0 || monthint > 12) {
                return new(false, $"month part of CPR '{monthint}' does not match an actual month");
            }
            if (dayint < 0 || dayint > DateTime.DaysInMonth(yearint, monthint)) {
                string monthName = (new DateTime(yearint, monthint, 1)).ToString("MMMM", CultureInfo.InvariantCulture);
                return new(false, $"{monthName} {yearint} did not contain a day {dayint}");
            }

            return new(true, "CPR is valid");
        }
        [HttpPut("{accountId}/cpr/{cpr}")]
        public async Task<IActionResult> UpdateCpr([FromRoute] int accountId, [FromRoute] string cpr) {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
            if (account is null) { return NotFound(accountId); }

            var validateResult = ValidateCprString(cpr);
            if (!validateResult.Valid) { return BadRequest(validateResult.Message); }

            byte[] hashBytes = await _hashingService.GetHashAsync(cpr);
            account.Cpr = Convert.ToBase64String(hashBytes);
            var addResult = _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
            return Ok("CPR updated succesfully");
        }

        [HttpPost("{accountId}/cpr/{cpr}")]
        public async Task<IActionResult> AuthenticateCpr([FromRoute] int accountId, [FromRoute] string cpr) {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
            if (account is null) { return NotFound(accountId); }

            var validateResult = ValidateCprString(cpr);
            if (!validateResult.Valid) { return BadRequest(validateResult.Message); }

            byte[] hashBytes = await _hashingService.GetHashAsync(cpr);
            string hashString = Convert.ToBase64String(hashBytes);

            if (!hashString.Equals(account.Cpr, StringComparison.InvariantCultureIgnoreCase)) {
                return Unauthorized("Incorrect CPR value");
            }

            return Ok("CPR valid!");
        }
    }
}
