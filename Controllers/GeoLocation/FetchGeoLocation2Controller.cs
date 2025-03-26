using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// This is a mock api for the third pary geoLocation provider

namespace ApiBroker.Controllers
{
    [Route("api/vendor2/get/location")]
    [ApiController]
    public class FetchGeoLocation2Controller : ControllerBase
    {
        private readonly ILogger<FetchGeoLocation2Controller> _logger;

        public FetchGeoLocation2Controller(ILogger<FetchGeoLocation2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetGeoLocation([FromQuery] string ipAddress)
        {
            // Mock data - Different from Vendor 1 to simulate another API
            var random = new Random();
            var mockData = new[]
            {
                new { IP = ipAddress, Country = "Canada", City = "Toronto"},
                new { IP = ipAddress, Country = "UK", City = "London"},
                new { IP = ipAddress, Country = "Australia", City = "Sydney"},
                new { IP = ipAddress, Country = "Brazil", City = "São Paulo"},
            };

            var response = mockData[random.Next(mockData.Length)];

            _logger.LogInformation("Returning mock geolocation data (Vendor 2) for IP: {IP}", ipAddress);
            return Ok(response);
        }
    }
}
