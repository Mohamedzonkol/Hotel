using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers
{
    public class VillaNumberController(IUnitOfWork unit) : Controller
    {
        public Task<IActionResult> Index()
        {
            var villaNumber =
                 unit.VillaNumberRepository.GetAllAsync(includeProperty: "Villa");
            return Task.FromResult<IActionResult>(View(villaNumber));

        }
        public async Task<IActionResult> Create()
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
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
        public async Task<IActionResult> Create(VillaNumberViewModel obj)
        {
            bool roomNumberExist = await unit.VillaNumberRepository.AnyAsync(x => x.Villa_Number == obj.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExist)
            {
                await unit.VillaNumberRepository.AddAsync(obj.VillaNumber);
                TempData["Success"] = "The Villa Number Has Been Created Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            else if (roomNumberExist)
            {
                TempData["Error"] = "The Villa Number Already Exist .";
                obj.VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                return View(obj);
            }
            obj.VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
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
                VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = await unit.VillaNumberRepository.GetAsync(x => x.Villa_Number == villaNumberId)
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
                await unit.VillaNumberRepository.UpdateAsync(obj.VillaNumber);

                TempData["Success"] = "The Villa Number Has Been Updated Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            obj.VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
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
                VillaList = unit.VillaRepository.GetAllAsync().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = await unit.VillaNumberRepository.GetAsync(x => x.Villa_Number == villaNumberId)
            };
            if (vm.VillaNumber == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Owner)]
        public async Task<IActionResult> Delete(VillaNumberViewModel obj)
        {
            var villaNumber = await unit.VillaNumberRepository.GetAsync(x => x.Villa_Number == obj.VillaNumber.Villa_Number);
            if (villaNumber is not null)
            {
                await unit.VillaNumberRepository.RemoveAsync(villaNumber);
                TempData["Success"] = "The Villa Number Has Been Deleted Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            TempData["Error"] = "The Villa Number Can Not Be Deleted .";

            return View();

        }
    }
}
