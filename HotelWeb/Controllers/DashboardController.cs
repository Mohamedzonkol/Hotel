using Hotel.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers
{
    public class DashboardController(IDashboardServices dashboardServices) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetTotalBookingRedialChartData()
        {
            return Json(await dashboardServices.GetTotalBookingRedialChartData());
        }
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            return Json(await dashboardServices.GetRegisteredUserChartData());
        }
        public async Task<IActionResult> GetRevenueChartData()
        {
            return Json(await dashboardServices.GetRevenueChartData());
        }
        public async Task<IActionResult> GetBookingPieChartData()
        {
            return Json(await dashboardServices.GetBookingPieChartData());
        }
        public async Task<IActionResult> GetMemberAndBookingLineChartData()
        {
            return Json(await dashboardServices.GetMemberAndBookingLineChartData());
        }
    }
}
