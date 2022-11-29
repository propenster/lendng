using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
