using Hotel.Domain.Entities;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Update(int villaId)
        {
            var villa = await context.Villas.FirstOrDefaultAsync(x => x.Id == villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                context.Villas.Update(villa);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Villa");
            }

            return View();
        }

        public async Task<IActionResult> Delete(int villaId)
        {
            var villa = await context.Villas.FirstOrDefaultAsync(x => x.Id == villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Villa villa)
        {
            var villaObj = await context.Villas.FirstOrDefaultAsync(x => x.Id == villa.Id);

            if (villaObj is not null)
            {
                context.Villas.Remove(villaObj);
                await context.SaveChangesAsync();
                return RedirectToAction("Index", "Villa");
            }

            return View();
        }
    }
}
