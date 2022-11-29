using AspNetCore.Identity.MongoDbCore.Models;
using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Models
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
    }
}
