using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelWeb.Controllers
{
    public class VillaController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            return View(context.Villas.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa villa)
        {
            if (villa.Name == villa.Description)
                ModelState.AddModelError("", "The Description Can Not Match The Name");

            if (ModelState.IsValid)
            {
                context.Villas.Add(villa);
                context.SaveChanges();
                return RedirectToAction("Index", "Villa");
            }
            return View();

        }
    }
}
