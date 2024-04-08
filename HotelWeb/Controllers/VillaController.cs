using Hotel.Application.Common.InterFaces;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace HotelWeb.Controllers
{
    public class VillaController(IUnitOfWork unit) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(unit.VillaRepository.GetAllAsync());
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Villa villa)
        {
            if (villa.Name == villa.Description)
                ModelState.AddModelError("", "The Description Can Not Match The Name");

            if (ModelState.IsValid)
            {
                await unit.VillaRepository.AddAsync(villa);
                TempData["Success"] = "The Villa Has Been Created Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Created .";
            return View();

        }
        public async Task<IActionResult> Update(int villaId)
        {
            var villa = await unit.VillaRepository.GetAsync(x => x.Id == villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                await unit.VillaRepository.UpdateAsync(villa);
                TempData["Success"] = "The Villa Has Been Updated Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Updated .";

            return View();
        }

        public async Task<IActionResult> Delete(int villaId)
        {
            var villa = await unit.VillaRepository.GetAsync(x => x.Id == villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Villa villa)
        {
            var villaObj = await unit.VillaRepository.GetAsync(x => x.Id == villa.Id);

            if (villaObj is not null)
            {
                await unit.VillaRepository.RemoveAsync(villaObj);
                TempData["Success"] = "The Villa Has Been Deleted Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Deleted .";
            return View();
        }


    }
}
