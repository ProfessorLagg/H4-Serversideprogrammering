using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() {
            throw new NotImplementedException();
        }
    }
}
