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
        private readonly ICircularProviderSelector _circularProviderSelector;
        private readonly int MAX_ATTEMPTS = 3;

        public ApiBrokerLogic(IHttpClientWrapper httpClientWrapper, IApiVitalsLogic apiVitalsLogic, ICircularProviderSelector circularProviderSelector)
        {
            _httpClientWrapper = httpClientWrapper;
            _apiVitalsLogic = apiVitalsLogic;
            _circularProviderSelector = circularProviderSelector;
        }

        public async Task<GeoLocationBrokerResponseDTO> GetGeoLocationLogic(string ipAddress, int attemps)
        {
            try{
                attemps++;
                var response = await TryGetGeoLocationLogic(ipAddress);
                return response;
            }catch(Exception _)
            {
                if(attemps >= MAX_ATTEMPTS )
                {
                    return new(ipAddress, "", "");
                }
                return await GetGeoLocationLogic(ipAddress, attemps);
            }
        }

        public async Task<GeoLocationBrokerResponseDTO> TryGetGeoLocationLogic(string ipAddress)
        {
            var provider = _circularProviderSelector.GetProvider();
            if(provider == null || provider == GeoLocationServiceProvider.INVALID_VENDOR)
            {
                throw new Exception("No providers available");
            }
            string requestUrl = GetProviderRequest((GeoLocationServiceProvider)provider) + $"?ipAddress={ipAddress}";

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            stopwatch.Stop();
            
            bool isError = response == null || string.IsNullOrWhiteSpace(response);
            long responseTime = stopwatch.ElapsedMilliseconds;
            
            _apiVitalsLogic.RecordResponse((GeoLocationServiceProvider)provider, responseTime, isError);

            if(isError)
            {
                throw new Exception("Api call error");
            }
            
            var result = JsonConvert.DeserializeObject<GeoLocationBrokerResponseDTO>(response);
            return result ?? throw new Exception("Could not deserialize response");;
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