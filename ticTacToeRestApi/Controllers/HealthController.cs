using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ticTacToeRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok();
    }
}
