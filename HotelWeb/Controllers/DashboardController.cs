using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
