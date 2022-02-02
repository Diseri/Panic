using AASA.NetCore.Lib.Helper.Models;
using Microsoft.AspNetCore.Mvc;

namespace AASA.NetCore.v1.Panic.Controllers
{
    [Route("v1/[controller]")]
    [Route("api/[controller]")] // Required for the Discovery Service, please do not remove.

    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var health = new HealthResponse()
            {
                Service = "Running",
                Status = "Healthy"
            };

            return Ok(health);
        }
    }
}