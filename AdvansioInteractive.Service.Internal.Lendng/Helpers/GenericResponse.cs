using Newtonsoft.Json;
using System.Net;

namespace AdvansioInteractive.Service.Internal.Lendng.Helpers
{
    public class GenericResponse<T>
    {
        [JsonProperty("data")]
        public T? Data { get; set; } 
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
