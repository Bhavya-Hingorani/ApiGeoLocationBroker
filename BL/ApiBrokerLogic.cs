using System.Text.Json.Serialization;
using ApiBroker.BL.Interfaces;
using ApiBroker.DTOs;
using Newtonsoft.Json;

namespace ApiBroker.BL
{
    public class ApiBrokerLogic : IApiBrokerLogic
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public ApiBrokerLogic(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<GeoLocationBrokerResponseDTO> GetGeoLocationLogic(string ipAddress)
        {
            string requestUrl = $"https://localhost:7185/api/vendor1/get/location/?ipAddress={ipAddress}";
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            if(response == null || string.IsNullOrWhiteSpace(response))
            {
                return new(ipAddress, "", "");
            }
            var result = JsonConvert.DeserializeObject<GeoLocationBrokerResponseDTO>(response);
            return result ?? new(ipAddress, "", "");
        }
    }
}