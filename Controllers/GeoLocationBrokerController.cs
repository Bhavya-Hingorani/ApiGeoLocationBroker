using System.Threading.Tasks;
using ApiBroker.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ApiBroker.Utils;

namespace ApiBroker.Controllers
{
    [Route("api/location-broker/")]
    [ApiController]
    public class GeoLocationBrokerController : ControllerBase
    {
        private readonly ILogger<GeoLocationBrokerController> _logger;
        private readonly IApiBrokerLogic _apiBrokerLogic;

        public GeoLocationBrokerController(IApiBrokerLogic apiBrokerLogic, ILogger<GeoLocationBrokerController> logger)
        {
            _apiBrokerLogic = apiBrokerLogic;
            _logger = logger;
        }

        [HttpGet("get/")]
        public async Task<IActionResult> GetGeoLocationUsingBroker([FromQuery] string ipAddress)
        {
            var (isValid, errMsg) = Validation.IsRequestValid(ipAddress);
            if (!isValid)
            {
                _logger.LogError(errMsg);
                return BadRequest(errMsg);
            }

            var response = await _apiBrokerLogic.GetGeoLocationLogic(ipAddress);
            if(response.IsValid)
            {
                return Ok(response);
            }
            _logger.LogWarning($"Broker failed to resolve IP: {ipAddress}");
            return StatusCode(502, response);
        }
    }
}
