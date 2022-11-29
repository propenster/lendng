using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests
{
    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress(ErrorMessage = "Email is invalid.")]
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ComparePassword { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
