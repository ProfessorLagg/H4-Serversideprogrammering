using API.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("debug")]
    [ApiController]
    public class DebugController : ControllerBase {
        private readonly H4serversideTodoContext DatabaseContext;
        private readonly IWebHostEnvironment _environment;
        public DebugController(H4serversideTodoContext dbContext, IWebHostEnvironment env) {
            this.DatabaseContext = dbContext;
            this._environment = env;
        }

        [HttpGet("WebRootPath")]
        public IActionResult GetWebRootPath() {
            return StatusCode(200, _environment.WebRootPath);
        }

    }
}
