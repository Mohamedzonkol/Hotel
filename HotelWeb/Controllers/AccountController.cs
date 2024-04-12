using Hotel.Application.Common.InterFaces;
using Hotel.Application.Utility;
using Hotel.Domain.Entities;
using HotelWeb.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HotelWeb.Controllers
{
    public class AccountController(IUnitOfWork unit, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        : Controller
    {

        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            LoginViewModel vm = new()
            {
                ReturnUrl = returnUrl
            };

            return View(vm);
        }
        public IActionResult Register(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).Wait();
            }

            RegisterViewModel vm = new()
            {
                RoleList = roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    Name = registerVM.UserName,
                    Email = registerVM.Email,
                    PhoneNumber = registerVM.PhoneNumber,
                    NormalizedEmail = registerVM.Email.ToUpper(),
                    EmailConfirmed = true,
                    UserName = registerVM.UserName,
                    CreatedAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerVM.Role))
                        await userManager.AddToRoleAsync(user, registerVM.Role);
                    else
                        await userManager.AddToRoleAsync(user, SD.Role_Customer);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    if (string.IsNullOrEmpty(registerVM.ReturnUrl))
                        return RedirectToAction("Index", "Home");
                    else
                        return LocalRedirect(registerVM.ReturnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            registerVM.RoleList = roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            });
            return View(registerVM);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel LoginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(LoginVM.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(LoginVM);
                }

                var result =
                    await signInManager.PasswordSignInAsync(user.Name, LoginVM.Password, LoginVM.RememberMe, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(LoginVM.ReturnUrl))
                        return RedirectToAction("Index", "Home");
                    else
                        return LocalRedirect(LoginVM.ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                }
            }
            return View(LoginVM);
        }
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
