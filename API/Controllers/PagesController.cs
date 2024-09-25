using API.Data;
using API.Data.Model;
using API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Text;

using static API.Services.H4AuthService;

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

        private ContentResult HtmlResult(string htmlString) {
            ContentResult result = new();
            result.ContentType = "text/html; charset=utf-8";
            result.Content = htmlString;
            result.StatusCode = 200;

            return result;
        }
        private async Task<IActionResult> GetPageContentResult(params string[] pathParts) {
            string? pageContent = await _pageContentService.GetPageContentAsync(pathParts);
            if (string.IsNullOrWhiteSpace(pageContent)) { return StatusCode(500, "Could not find login page content"); }
            return HtmlResult(pageContent);
        }

        private async Task<IActionResult> ReturnIfAuthorized(Task<IActionResult> pageTask) {
            string? authHeader = Request.Headers.Authorization;
            if (authHeader is null) { return await LoginPage(); }
            ValidateSessionResult validateResult = await _authService.ValidateSession(authHeader);
            if (validateResult is null || validateResult.Session is null) { return await LoginPage(); }
            if (!validateResult.Valid) { return await LoginPage(); }

            if (!validateResult.Session.SecondFactorAuthenticated) { return await CprPage(); }

            return await pageTask;
        }

        [HttpGet("login")]
        public async Task<IActionResult> LoginPage() {
            return await GetPageContentResult("login");
        }

        [HttpGet("home")]
        public async Task<IActionResult> HomePage() {
            return await ReturnIfAuthorized(GetPageContentResult("home"));
        }

        [HttpGet("cpr")]
        public async Task<IActionResult> CprPage() {
            string? authHeader = Request.Headers.Authorization;
            if (authHeader is null) { return await LoginPage(); }

            ValidateSessionResult validateResult = await _authService.ValidateSession(authHeader);
            if (validateResult is null) { throw new ArgumentNullException(nameof(validateResult)); }
            if (!validateResult.Valid) { return await LoginPage(); }

            AccountSession? session = validateResult.Session;
            if (validateResult.Session is null) { throw new ArgumentNullException(nameof(validateResult.Session)); }

            Account? account = validateResult.Session.Account;
            if (account is null) {
                account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == validateResult.Session.AccountId);
                if (account is null) {
                    throw new ArgumentNullException(nameof(validateResult.Session.Account));
                }
            }

            return await GetPageContentResult("cpr");
        }

        [HttpGet("todo")]
        public async Task<IActionResult> TodoItemList() {
            string? authHeader = Request.Headers.Authorization;
            if (authHeader is null) { return await LoginPage(); }

            ValidateSessionResult validateResult = await _authService.ValidateSession(authHeader);
            if (validateResult is null) { throw new ArgumentNullException(nameof(validateResult)); }
            if (!validateResult.Valid) { return await LoginPage(); }

            AccountSession? session = validateResult.Session;
            if (validateResult.Session is null) { throw new ArgumentNullException(nameof(validateResult.Session)); }

            Account account = (await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == validateResult.Session.AccountId))!;
            IList<TodoItem> todoItems = _dbContext
                .TodoItems
                .Where(x => x.AccountId == account.Id)
                .ToArray();

            StringBuilder sb = new();
            foreach (TodoItem todoItem in todoItems) {
                sb.Append("<tr><td>");
                sb.Append(todoItem.Title);
                sb.Append("</td></tr>");
            }

            return HtmlResult(sb.ToString());
        }
    }
}
