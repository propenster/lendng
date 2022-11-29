using AdvansioInteractive.Service.Internal.Lendng.Models;
using AdvansioInteractive.Service.Internal.Lendng.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;

namespace AdvansioInteractive.Service.Internal.Lendng.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            var client = _httpClientFactory.CreateClient("LendngClient");
            var loginUrl = Url.Action("Login", "Authentication");
            var stringResult = string.Empty;
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            var payload = JsonConvert.SerializeObject(new {Email = model.Email, Password = model.Password});
            requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            var res = await client.SendAsync(requestMessage);
            stringResult = await res.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<GenericResponse<LoginResponse>>(stringResult);
            if (!loginResponse.Success)
            {
                ViewData["ResponseMessage"] = loginResponse?.Message;
                return View(model);
            }

            return View(model);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}