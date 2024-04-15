using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HotelWeb.Controllers
{
    [Authorize]
    public class VillaController(IVillaServices villaServices) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(villaServices.GetAllVilla());
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
                await villaServices.CreateAsync(villa);
                TempData["Success"] = "The Villa Has Been Created Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Created .";
            return View();

        }
        public async Task<IActionResult> Update(int villaId)
        {
            var villa = await villaServices.GetVillaById(villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                await villaServices.UpdateAsync(villa);
                TempData["Success"] = "The Villa Has Been Updated Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Updated .";

            return View();
        }
        public async Task<IActionResult> Delete(int villaId)
        {
            var villa = await villaServices.GetVillaById(villaId);
            if (villa == null)
                return RedirectToAction("Error", "Home");
            return View(villa);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Delete(Villa villa)
        {
            bool isDeleted = await villaServices.Delete(villa.Id);
            if (isDeleted)
            {
                TempData["Success"] = "The Villa Has Been Deleted Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Deleted .";
            return View();
        }
    }
}
