using Microsoft.AspNetCore.Mvc;

namespace ShippingAndLogisticsManagement.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
