using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Helpers
{
    public class DefaultErrorResponse
    {
        //[RegularExpression(@"(?:^.*[^a-zA-Z0-9]|^)")]
        [Required, RegularExpression(@"^.*")]
        public string Error { get; set; } = string.Empty;   

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
