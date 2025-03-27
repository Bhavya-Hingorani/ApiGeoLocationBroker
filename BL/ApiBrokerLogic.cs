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
            var provider = DynamicRouting();
            string requestUrl = GetProviderRequest(provider) + $"?ipAddress={ipAddress}";

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            stopwatch.Stop();
            
            bool isError = response == null || string.IsNullOrWhiteSpace(response);
            long responseTime = stopwatch.ElapsedMilliseconds;
            
            _apiVitalsLogic.RecordResponse(provider, responseTime, isError);

            if(isError)
            {
                return new(ipAddress, "", "");
            }
            
            var result = JsonConvert.DeserializeObject<GeoLocationBrokerResponseDTO>(response);
            return result ?? new(ipAddress, "", "");
        }

        private GeoLocationServiceProvider DynamicRouting()
        {
            var allProviders = _apiVitalsLogic.GetAllApiVitalsStateValues();

            var greenProvider = allProviders.FirstOrDefault(kv => (kv.Value ?? new()).ApiVitalsState == ApiVitalsState.GREEN).Key;
            if (greenProvider != default)
                return greenProvider;

            var orangeProvider = allProviders.FirstOrDefault(kv => (kv.Value ?? new()).ApiVitalsState == ApiVitalsState.ORANGE).Key;
            if(orangeProvider != default)
            {
                return orangeProvider;
            }
            // LOG error here
            return GeoLocationServiceProvider.INVALID_VENDOR;
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