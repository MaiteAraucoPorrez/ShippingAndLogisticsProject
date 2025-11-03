using Microsoft.AspNetCore.Mvc;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    [Route("api/v{version:ApiVersion}/shipment")]
    [ApiVersion("2.0")]
    [ApiController]
    public class ShipmentV2Controller : Controller
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
