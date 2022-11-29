using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class CoreController : ControllerBase
    {

        private readonly ICoreService _coreService;
        private readonly ILogger _logger;

        public CoreController(ICoreService coreService, ILogger<CoreController> logger)
        {
            _coreService = coreService;
            _logger = logger;
        }

        [HttpPost, Route("fundtransfer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GenericResponse<FundTransferResponse>))]
        [SwaggerOperation(Summary = "Do fund transfer")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType, Type = typeof(GenericResponse<FundTransferResponse>))]
        [ProducesDefaultResponseType(typeof(DefaultErrorResponse))]
        public async Task<IActionResult> FundTransfer([FromBody] FundTransferRequest request)
        {
            //var result = await _payAttitudeService.GetStatementAsync(request);
            _logger.LogInformation($"{nameof(FundTransfer)} Fund Transfer Request: {request.ToString()}");
            string IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{nameof(FundTransfer)} IP is {IPAddress}");
            //var result = await _advancioService.InterbankTransfer(IPAddress, request);
            var response = await _coreService.FundTransfer(request);
            _logger.LogInformation($"{nameof(FundTransfer)} Fund Transfer Response {JsonConvert.SerializeObject(response)} AT TIMESTAMPS: {DateTime.Now}");

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost, Route("balanceenquiry")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(GenericResponse<decimal>))]
        [SwaggerOperation(Summary = "Get Balance Enquiry")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(GenericResponse<decimal>))]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType, Type = typeof(GenericResponse<decimal>))]
        [ProducesDefaultResponseType(typeof(DefaultErrorResponse))]
        public async Task<IActionResult> GetBalance([FromBody] BalanceEnquiryRequest request)
        {
            //var result = await _payAttitudeService.GetStatementAsync(request);
            _logger.LogInformation($"{nameof(GetBalance)} Get Balance Request: {request.ToString()}");
            string IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation($"{nameof(GetBalance)} IP is {IPAddress}");
            //var result = await _advancioService.InterbankTransfer(IPAddress, request);
            var response = await _coreService.GetBalance(request.WalletId);
            _logger.LogInformation($"{nameof(GetBalance)} Get Balance Response {JsonConvert.SerializeObject(response)} AT TIMESTAMPS: {DateTime.Now}");
            return StatusCode((int)response.StatusCode, response);
        }

    }
}
