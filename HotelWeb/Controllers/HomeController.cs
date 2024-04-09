using Hotel.Application.Common.InterFaces;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers
{
    public class HomeController(IUnitOfWork unit) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeViewModel vm = new()
            {
                VillaList = unit.VillaRepository.GetAllAsync(includeProperty: "VillaAmenities"),
                Nights = 1,
                CheckInData = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
