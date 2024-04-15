using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HotelWeb.Controllers
{
    [Authorize]
    public class VillaController(IUnitOfWork unit, IWebHostEnvironment webHost) : Controller
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
                if (villa.Image is not null)
                {
                    string fileName =
                        Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(webHost.WebRootPath, @"Images\Villa");
                    await using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    await villa.Image.CopyToAsync(fileStream);
                    villa.ImageUrl = @"\Images\Villa\" + fileName;
                }
                else
                {
                    villa.ImageUrl = "https://placehold.co/600x400"; //default image
                }
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
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                if (villa.Image is not null)
                {
                    string fileName =
                        Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                    string imagePath = Path.Combine(webHost.WebRootPath, @"Images\Villa");
                    if (!string.IsNullOrEmpty(villa.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(webHost.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }
                    await using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    await villa.Image.CopyToAsync(fileStream);
                    villa.ImageUrl = @"\Images\Villa\" + fileName;
                }
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
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Delete(Villa villa)
        {
            var villaObj = await unit.VillaRepository.GetAsync(x => x.Id == villa.Id);

            if (villaObj is not null)
            {
                if (!string.IsNullOrEmpty(villaObj.ImageUrl))
                {
                    string oldImagePath = Path.Combine(webHost.WebRootPath, villaObj.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                await unit.VillaRepository.RemoveAsync(villaObj);
                TempData["Success"] = "The Villa Has Been Deleted Successfully .";
                return RedirectToAction("Index", "Villa");
            }
            TempData["Error"] = "The Villa Can Not Be Deleted .";
            return View();
        }


    }
}
