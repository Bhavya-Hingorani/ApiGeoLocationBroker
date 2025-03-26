using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// This is a mock api for the third pary geoLocation provider

namespace ApiBroker.Controllers
{
    [Route("api/vendor1/get/location")]
    [ApiController]
    public class FetchGeoLocation1Controller : ControllerBase
    {
        private readonly ILogger<FetchGeoLocation1Controller> _logger;

        public FetchGeoLocation1Controller(ILogger<FetchGeoLocation1Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetGeoLocation([FromQuery] string ipAddress)
        {
            // Mock data
            var random = new Random();
            var mockData = new[]
            {
                new { IP = ipAddress, Country = "India", City = "Mumbai"},
                new { IP = ipAddress, Country = "USA", City = "New York"},
                new { IP = ipAddress, Country = "Germany", City = "Berlin"},
                new { IP = ipAddress, Country = "Japan", City = "Tokyo"},
            };

            var response = mockData[random.Next(mockData.Length)];

            _logger.LogInformation("Returning mock geolocation data for IP: {IP}", ipAddress);
            return Ok(response);
        }
    }
}
