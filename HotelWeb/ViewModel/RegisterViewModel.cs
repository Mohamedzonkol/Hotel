using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelWeb.ViewModel
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Congirm Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        [DataType(DataType.PhoneNumber), Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        public string? ReturnUrl { get; set; }
        public string? Role { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
