using Microsoft.AspNetCore.Mvc;

namespace ApiBroker.Controllers
{
    [Route("api/location-broker/")]
    [ApiController]
    public class GeoLocationBrokerController : ControllerBase
    {
        private readonly ILogger<GeoLocationBrokerController> _logger;

        public GeoLocationBrokerController(ILogger<GeoLocationBrokerController> logger)
        {
            _logger = logger;
        }

        [HttpGet("get/")]
        public IActionResult GetGeoLocationUsingBroker([FromQuery] string ipAddress)
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

            return Ok(dummyResponse);
        }
    }
}
