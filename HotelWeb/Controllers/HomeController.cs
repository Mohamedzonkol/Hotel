using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers
{
    public class HomeController(IUnitOfWork unit) : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel vm = new()
            {
                VillaList = unit.VillaRepository.GetAllAsync(includeProperty: "VillaAmenities"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            Thread.Sleep(500);
            var villaList = unit.VillaRepository.GetAllAsync(includeProperty: "VillaAmenities").ToList();
            var villaNumberList = unit.VillaNumberRepository.GetAllAsync().ToList();
            var bookedVillas = unit.BookingRepository.GetAllAsync(x => x.Status == SD.StatusApproved
            || x.Status == SD.StatusCheckedIn).ToList();
            foreach (var villa in villaList)
            {
                int roomAvailable =
                    SD.VillaRoomsAvailable_Count(villa.Id, villaNumberList, checkInDate, nights, bookedVillas);
                villa.IsAvailable = roomAvailable > 0 ? true : false;
            }

            HomeViewModel homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
                Nights = nights
            };
            return PartialView("_VillaList", homeVM);
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
