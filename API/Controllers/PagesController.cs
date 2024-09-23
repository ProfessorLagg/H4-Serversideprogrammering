﻿using API.Data;
using API.Services;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("pages")]
    [ApiController]
    public class PagesController : ControllerBase {

        private readonly H4serversideTodoContext _dbContext;
        private readonly IPageContentService _pageContentService;
        private readonly H4AuthService _authService;
        public PagesController(H4serversideTodoContext dbContext, IPageContentService pageContentService) {
            this._dbContext = dbContext;
            this._pageContentService = pageContentService;
            this._authService = new(this._dbContext);
        }

        private bool IsAuthenticated(string sessionToken) {
            return true; // TODO Check for active session in DB
        }
        private async Task<IActionResult> GetPageContentResult(params string[] pathParts) {
            string? pageContent = await _pageContentService.GetPageContentAsync(pathParts);

            if (string.IsNullOrWhiteSpace(pageContent)) { return StatusCode(500, "Could not find login page content"); }

            ContentResult result = new();
            result.ContentType = "text/html; charset=utf-8";
            result.Content = pageContent;
            result.StatusCode = 200;

            return result;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginPage() {
            return await GetPageContentResult("Login");
        }

        [HttpGet("home")]
        public async Task<IActionResult> HomePage() {
            string? authHeader = Request.Headers.Authorization;
            if (authHeader is null) { return await LoginPage(); }
            string tokenString = authHeader.Substring(authHeader.IndexOf(' ') + 1);
            Guid token = Guid.Parse(tokenString);
            bool validToken = await _authService.ValidateSessionToken(token);
            if (!validToken) { return await LoginPage(); }


            return await GetPageContentResult("Home");
        }

    }
}
