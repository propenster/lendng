using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthenticationController  : ControllerBase
    {

        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        public AuthenticationController(IAccountService accountService, ILogger<AuthenticationController> logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<RegisterResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GenericResponse<RegisterResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GenericResponse<RegisterResponse>))]
        [SwaggerOperation(Summary = "To register a new user")]
        [ProducesDefaultResponseType(typeof(DefaultErrorResponse))]
        public async Task<ActionResult<GenericResponse<LoginResponse>>> Register([FromBody] RegisterDto user)
        {
            _logger.LogInformation($"{nameof(Register)} Register Request: {user.ToString()}");
            string IPAddress = Request?.HttpContext?.Connection?.RemoteIpAddress.ToString();
            _logger.LogInformation($"{nameof(Register)} IP is {IPAddress}");
            var loggedUser = await _accountService.Register(user);

            _logger.LogInformation($"{nameof(Register)} Register Response {JsonConvert.SerializeObject(loggedUser)} AT TIMESTAMPS: {DateTime.Now}");

            return loggedUser.Success switch
            {
                false => StatusCode((int)loggedUser.StatusCode, loggedUser),
                _ => Ok(loggedUser)
            };
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<LoginResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GenericResponse<string>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GenericResponse<string>))]

        [SwaggerOperation(Summary = "To login as a user")]
        [ProducesDefaultResponseType(typeof(DefaultErrorResponse))]
        public async Task<ActionResult<GenericResponse<LoginResponse>>> Login([FromBody] LoginDto user)
        {
            _logger.LogInformation($"{nameof(Login)} Login Request: {user.ToString()}");
            string IPAddress = Request?.HttpContext?.Connection?.RemoteIpAddress.ToString();
            _logger.LogInformation($"{nameof(Login)} IP is {IPAddress}");
            var loggedUser = await _accountService.Login(user);

            _logger.LogInformation($"{nameof(Login)} Login Response {JsonConvert.SerializeObject(loggedUser)} AT TIMESTAMPS: {DateTime.Now}");

            return loggedUser.Success switch
            {
                false => StatusCode((int)loggedUser.StatusCode, loggedUser),
                _ => Ok(loggedUser)
            };
        }

    }
}
