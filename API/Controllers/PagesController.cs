using API.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("pages")]
    [ApiController]
    public class PagesController : ControllerBase {

        private readonly H4serversideTodoContext DatabaseContext;

        public PagesController(H4serversideTodoContext dbContext) {
            this.DatabaseContext = dbContext;
        }

        private bool IsAuthenticated(string sessionToken) {
            return true; // TODO Check for active session in DB
        }

        //private static readonly ContentResult LoginPageResult = new ContentResult {
        //    Content = API.Properties.Resources.PageLogin,
        //    ContentType = "text/html; charset=utf-8",
        //    StatusCode = 200,
        //};

        //[HttpGet("login")]
        //public async Task<IActionResult> LoginPage() {
        //    ContentResult result = LoginPageResult;
        //    return LoginPageResult;
        //}

        //private static readonly ContentResult HomePageResult = new ContentResult {
        //    Content = API.Properties.Resources.PageHome,
        //    ContentType = "text/html; charset=utf-8",
        //    StatusCode = 200,
        //};
        //[HttpGet("home")]
        //public async Task<IActionResult> HomePage() {
        //    string? authHeader = Request.Headers.Authorization;
        //    if (authHeader is null || !IsAuthenticated(authHeader)) { return await LoginPage(); }
        //    return HomePageResult;
        //}

    }
}
