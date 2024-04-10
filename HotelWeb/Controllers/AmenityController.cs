using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController(IUnitOfWork unit) : Controller
    {
        public Task<IActionResult> Index()
        {
            var amenities =
                unit.AmenityRepository.GetAllAsync(includeProperty: "Villa");
            return Task.FromResult<IActionResult>(View(amenities));
        }
        public async Task<IActionResult> Create()
        {
            AmenityViewModel vm = new AmenityViewModel
            {
                VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AmenityViewModel obj)
        {
            if (ModelState.IsValid)
            {
                await unit.AmenityRepository.AddAsync(obj.Amenity);
                TempData["Success"] = "The Amenity Has Been Created Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            obj.VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Amenity Can Not Be Created .";
            return View(obj);
        }
        public async Task<IActionResult> Update(int amentityId)
        {
            AmenityViewModel vm = new AmenityViewModel
            {
                VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = await unit.AmenityRepository.GetAsync(x => x.Id == amentityId)
            };
            if (vm.Amenity == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(AmenityViewModel obj)
        {
            if (ModelState.IsValid)
            {
                await unit.AmenityRepository.UpdateAsync(obj.Amenity);
                TempData["Success"] = "The Amenity Has Been Updated Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            obj.VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Amenity Can Not Be Updated .";
            return View(obj);
        }
        public async Task<IActionResult> Delete(int amentityId)
        {
            AmenityViewModel vm = new AmenityViewModel
            {
                VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = await unit.AmenityRepository.GetAsync(x => x.Id == amentityId)
            };
            if (vm.Amenity == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(AmenityViewModel obj)
        {
            var amenity = await unit.AmenityRepository.GetAsync(x => x.Id == obj.Amenity.Id);
            if (amenity is not null)
            {
                await unit.AmenityRepository.RemoveAsync(amenity);
                TempData["Success"] = "The Amenity Has Been Deleted Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            TempData["Error"] = "The Amenity Can Not Be Deleted .";
            return View();

        }
    }
}
