using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests
{
    public class LoginDto
    {
        [Required, JsonProperty("email")]
        public string Email { get; set; } = string.Empty;
        [Required, JsonProperty("password")]
        public string Password { get; set; } = string.Empty;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
