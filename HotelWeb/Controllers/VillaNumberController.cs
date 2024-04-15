using Hotel.Application.Services.Interfaces;
using Hotel.Application.Utility;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers
{
    public class VillaNumberController(IVIllaNumberServices villaNumberService, IVillaServices villaServices) : Controller
    {
        public Task<IActionResult> Index()
        {
            var villaNumber =
                 villaNumberService.GetAllVillaNumber("Villa");
            return Task.FromResult<IActionResult>(View(villaNumber));

        }
        public async Task<IActionResult> Create()
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
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
        public async Task<IActionResult> Create(VillaNumberViewModel obj)
        {
            bool roomNumberExist = await villaNumberService.CheckVillaNumberExist(obj.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExist)
            {
                await villaNumberService.CreateVillaNumberAsync(obj.VillaNumber);
                TempData["Success"] = "The Villa Number Has Been Created Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            else if (roomNumberExist)
            {
                TempData["Error"] = "The Villa Number Already Exist .";
                obj.VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                return View(obj);
            }
            obj.VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Villa Number Can Not Be Created .";
            return View(obj);

        }
        public async Task<IActionResult> Update(int villaNumberId)
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
            {
                VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = await villaNumberService.GetVillaNumberById(villaNumberId)
            };
            if (vm.VillaNumber == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Update(VillaNumberViewModel obj)
        {
            if (ModelState.IsValid)
            {
                await villaNumberService.UpdateAsync(obj.VillaNumber);
                TempData["Success"] = "The Villa Number Has Been Updated Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            obj.VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Villa Number Can Not Be Updated .";
            return View(obj);
        }
        public async Task<IActionResult> Delete(int villaNumberId)
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
            {
                VillaList = villaServices.GetAllVilla().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = await villaNumberService.GetVillaNumberById(villaNumberId)
            };
            if (vm.VillaNumber == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Delete(VillaNumberViewModel obj)
        {
            var villaNumber = await villaNumberService.GetVillaNumberById(obj.VillaNumber.Villa_Number);
            if (villaNumber is not null)
            {
                await villaNumberService.Delete(villaNumber.Villa_Number);
                TempData["Success"] = "The Villa Number Has Been Deleted Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            TempData["Error"] = "The Villa Number Can Not Be Deleted .";

            return View();

        }
    }
}
