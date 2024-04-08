using Hotel.Infrastructure.Data;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelWeb.Controllers
{
    public class VillaNumberController(ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            return View(context.VillaNumbers
                .Include(x => x.Villa).ToList());
        }
        public IActionResult Create()
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
            {
                VillaList = context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(VillaNumberViewModel obj)
        {
            bool roomNumberExist = context.VillaNumbers.Any(x => x.Villa_Number == obj.VillaNumber.Villa_Number);
            if (ModelState.IsValid && !roomNumberExist)
            {
                context.VillaNumbers.Add(obj.VillaNumber);
                context.SaveChanges();
                TempData["Success"] = "The Villa Number Has Been Created Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            else if (roomNumberExist)
            {
                TempData["Error"] = "The Villa Number Already Exist .";
                obj.VillaList = context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                return View(obj);
            }
            obj.VillaList = context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Villa Number Can Not Be Created .";
            return View(obj);

        }
        public IActionResult Update(int villaNumberId)
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
            {
                VillaList = context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)
            };
            if (vm.VillaNumber == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberViewModel obj)
        {
            if (ModelState.IsValid)
            {
                context.VillaNumbers.Update(obj.VillaNumber);
                context.SaveChanges();
                TempData["Success"] = "The Villa Number Has Been Updated Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            obj.VillaList = context.Villas.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
            TempData["Error"] = "The Villa Number Can Not Be Updated .";
            return View(obj);
        }
        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberViewModel vm = new VillaNumberViewModel
            {
                VillaList = context.Villas.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                VillaNumber = context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == villaNumberId)
            };
            if (vm.VillaNumber == null)
                return RedirectToAction("Error", "Home");
            return View(vm);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumberViewModel obj)
        {
            var villaNumber = context.VillaNumbers.FirstOrDefault(x => x.Villa_Number == obj.VillaNumber.Villa_Number);
            if (villaNumber is not null)
            {
                context.VillaNumbers.Remove(villaNumber);
                context.SaveChanges();
                TempData["Success"] = "The Villa Number Has Been Deleted Successfully .";
                return RedirectToAction("Index", "VillaNumber");
            }
            TempData["Error"] = "The Villa Number Can Not Be Deleted .";

            return View();

        }
    }
}
