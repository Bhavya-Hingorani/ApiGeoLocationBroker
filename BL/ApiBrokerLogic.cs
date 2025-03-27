using System.Text.Json.Serialization;
using ApiBroker.BL.Interfaces;
using ApiBroker.DTOs;
using ApiBroker.Entities;
using ApiBroker.Entities.Enum;
using Newtonsoft.Json;

namespace ApiBroker.BL
{
    public class ApiBrokerLogic : IApiBrokerLogic
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IApiVitalsLogic _apiVitalsLogic;

        public ApiBrokerLogic(IHttpClientWrapper httpClientWrapper, IApiVitalsLogic apiVitalsLogic)
        {
            _httpClientWrapper = httpClientWrapper;
            _apiVitalsLogic = apiVitalsLogic;
        }

        public async Task<GeoLocationBrokerResponseDTO> GetGeoLocationLogic(string ipAddress)
        {
            string requestUrl = DynamicRouting() + $"?ipAddress={ipAddress}";
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            if(response == null || string.IsNullOrWhiteSpace(response))
            {
                return new(ipAddress, "", "");
            }
            var result = JsonConvert.DeserializeObject<GeoLocationBrokerResponseDTO>(response);
            return result ?? new(ipAddress, "", "");
        }

        private string DynamicRouting()
        {
            var allProviders = _apiVitalsLogic.GetAllApiVitalsStateValues();

            var greenProvider = allProviders.FirstOrDefault(kv => (kv.Value ?? new()).ApiVitalsState == ApiVitalsState.GREEN).Key;
            if (greenProvider != default)
                return GetProviderRequest(greenProvider);

            var orangeProvider = allProviders.FirstOrDefault(kv => (kv.Value ?? new()).ApiVitalsState == ApiVitalsState.ORANGE).Key;
            if(orangeProvider != default)
            {
                return GetProviderRequest(orangeProvider);
            }
            // LOG error here
            return string.Empty;
        }

        private string GetProviderRequest(GeoLocationServiceProvider provider)
        {
            switch(provider)
            {
                case GeoLocationServiceProvider.VENDOR_ONE:
                    return "https://localhost:7185/api/vendor1/get/location/";
                case GeoLocationServiceProvider.VENDOR_TWO:
                    return "https://localhost:7185/api/vendor2/get/location/";
                case GeoLocationServiceProvider.VENDOR_THREE:
                    return "https://localhost:7185/api/vendor3/get/location/";
                default:
                    return string.Empty;
            }
        }
    }
}