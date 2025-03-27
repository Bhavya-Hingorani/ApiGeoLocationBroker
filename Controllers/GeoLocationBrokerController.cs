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
            if (string.IsNullOrWhiteSpace(ipAddress) || !System.Net.IPAddress.TryParse(ipAddress, out _))
            {
                _logger.LogError("Invalid IP Address format: {Ip}", ipAddress);
                return BadRequest("Invalid IP Address");
            }

            var response = await _apiBrokerLogic.GetGeoLocationLogic(ipAddress, 0);
            return Ok(response);
        }
    }
}
