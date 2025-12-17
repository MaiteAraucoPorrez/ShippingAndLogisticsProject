using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    /// <summary>
    /// Version 2 del controlador de envíos
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:ApiVersion}/shipment")]
    [Produces("application/json")]
    public class ShipmentV2Controller : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                Version = "2.0",
                Message = "Shipment API v2"
            });
        }
    }
}