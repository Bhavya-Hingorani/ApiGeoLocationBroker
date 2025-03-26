using System.Threading.Tasks;
using ApiBroker.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                _logger.LogError("Invalid IP Address");
                return BadRequest("Invalid IP Address");
            }

            var dummyResponse = new
            {
                IP = ipAddress,
                Country = "India",
                City = "Mumbai"
            };
            
            await _apiBrokerLogic.GetGeoLocationLogic(ipAddress);

            return Ok(dummyResponse);
        }
    }
}
