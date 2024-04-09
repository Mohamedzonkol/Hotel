using System.ComponentModel.DataAnnotations;

namespace HotelWeb.ViewModel
{
    public class LoginViewModel
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

    }
}
