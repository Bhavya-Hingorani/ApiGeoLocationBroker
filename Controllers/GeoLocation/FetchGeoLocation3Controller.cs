using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// This is a mock api for the third pary geoLocation provider

namespace ApiBroker.Controllers
{
    [Route("api/vendor3/get/location")]
    [ApiController]
    public class FetchGeoLocation3Controller : ControllerBase
    {
        private readonly ILogger<FetchGeoLocation3Controller> _logger;

        public FetchGeoLocation3Controller(ILogger<FetchGeoLocation3Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetGeoLocation([FromQuery] string ipAddress)
        {
            // Mock data - Unique to vendor 3
            var random = new Random();
            var mockData = new[]
            {
                new { IP = ipAddress, Country = "France", City = "Paris"},
                new { IP = ipAddress, Country = "Russia", City = "Moscow"},
                new { IP = ipAddress, Country = "China", City = "Beijing"},
                new { IP = ipAddress, Country = "South Africa", City = "Cape Town"},
            };

            var response = mockData[random.Next(mockData.Length)];

            _logger.LogInformation("Returning mock geolocation data (Vendor 3) for IP: {IP}", ipAddress);
            return Ok(response);
        }
    }
}
