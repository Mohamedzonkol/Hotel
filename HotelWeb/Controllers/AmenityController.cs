using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AmenityController(IAmenityServices amenityServices, IVillaServices villaServices) : Controller
    {
        public Task<IActionResult> Index()
        {
            var amenities =
                amenityServices.GetAllAmenity("Villa");
            return Task.FromResult<IActionResult>(View(amenities));
        }
        public async Task<IActionResult> Create()
        {
            AmenityViewModel vm = new AmenityViewModel
            {
                VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
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
                await amenityServices.CreateAsync(obj.Amenity);
                TempData["Success"] = "The Amenity Has Been Created Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            obj.VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
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
                VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = await amenityServices.GetAmenityById(amentityId)
            };
            if (vm.Amenity == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Update(AmenityViewModel obj)
        {
            if (ModelState.IsValid)
            {
                await amenityServices.UpdateAsync(obj.Amenity);
                TempData["Success"] = "The Amenity Has Been Updated Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            obj.VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
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
                VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                Amenity = await amenityServices.GetAmenityById(amentityId)
            };
            if (vm.Amenity == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Delete(AmenityViewModel obj)
        {
            var amenity = await amenityServices.GetAmenityById(obj.Amenity.Id);
            if (amenity is not null)
            {
                await amenityServices.Delete(amenity.Id);
                TempData["Success"] = "The Amenity Has Been Deleted Successfully .";
                return RedirectToAction("Index", "Amenity");
            }
            TempData["Error"] = "The Amenity Can Not Be Deleted .";
            return View();

        }
    }
}
