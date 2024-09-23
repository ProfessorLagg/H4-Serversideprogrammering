﻿using API.Data;
using API.Data.Model;
using API.Services;
using API.Utils.HashingUtils;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

using static API.Services.H4AuthService;

namespace API.Controllers {
    [Route("[controller]")]
    [ApiController]
    public sealed class AccountController : ControllerBase {

        private readonly H4serversideTodoContext _dbContext;
        private readonly H4AuthService authService;


        public AccountController(H4serversideTodoContext dbContext) {
            this._dbContext = dbContext;
            this.authService = new(this._dbContext);
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
            public int AccountId { get; set; } = -1;
            public bool Authenticated { get; set; } = false;
            public Guid SessionToken { get; set; } = Guid.Empty;
            public string Message { get; set; } = string.Empty;
            public AuthenticateResponse() { }
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
            if (string.IsNullOrWhiteSpace(request.PasswordHash)) {
                resp.Message = "passwordHash cannot be null or empty";
                return BadRequest(resp);
            }

            AuthResult authResult = await authService.AuthenticateAsync(request.Login, request.PasswordHash);
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

            HashAlgorithm hasher = SHA384.Create();

            Account account = new();
            account.Login = request.Login;

            // TODO Check password Requirements
            account.PasswordHash = Convert.ToHexString(hasher.ComputeHash(request.Password)).ToLower();
            account.Cpr = null;
            var addResult = await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();


            Account outAccount = addResult.Entity;

            //request = null!;
            //System.GC.Collect(0);
            CreateUserResponse response = new(outAccount.Id, outAccount.Login);
            return Ok(response);
        }
    }
}
