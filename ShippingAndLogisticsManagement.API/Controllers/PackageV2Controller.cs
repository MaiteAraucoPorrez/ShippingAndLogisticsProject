using Microsoft.AspNetCore.Mvc;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:ApiVersion}/package")]
    public class PackageV2Controller : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = 2.0,
                Message = "Version 2"
            });
        }
    }
}
