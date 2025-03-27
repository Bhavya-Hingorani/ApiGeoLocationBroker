using ApiBroker.BL.Interfaces;
using ApiBroker.DTOs;
using ApiBroker.Entities.Enum;
using ApiBroker.Utils;
using Newtonsoft.Json;

namespace ApiBroker.BL
{
    public class ApiBrokerLogic : IApiBrokerLogic
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly IApiVitalsLogic _apiVitalsLogic;
        private readonly ICircularProviderSelector _circularProviderSelector;
        private readonly IConfiguration _config;
        private readonly ILogger<ApiBrokerLogic> _logger;

        public ApiBrokerLogic(IHttpClientWrapper httpClientWrapper, IApiVitalsLogic apiVitalsLogic, ICircularProviderSelector circularProviderSelector, IConfiguration config, ILogger<ApiBrokerLogic> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _apiVitalsLogic = apiVitalsLogic;
            _circularProviderSelector = circularProviderSelector;
            _config = config;
            _logger = logger;
        }

        public async Task<GeoLocationBrokerResponseDTO> GetGeoLocationLogic(string ipAddress)
        {
            int attempts = 0;
            while(attempts < Constants.MAX_ATTEMPS)
            {
                try
                {
                    return await TryGetGeoLocationLogic(ipAddress);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                    attempts++;
                }            
            }
            string msg = $"attempts exceeding while getting geoLocation for {ipAddress}";
            _logger.LogError(msg);
            return new(ipAddress, string.Empty, string.Empty, msg);
        }

        public async Task<GeoLocationBrokerResponseDTO> TryGetGeoLocationLogic(string ipAddress)
        {
            var provider = _circularProviderSelector.GetProvider();
            if(provider == null || provider == GeoLocationServiceProvider.INVALID_VENDOR)
            {
                throw new Exception("No providers available");
            }
            string requestUrl = GetProviderRequest((GeoLocationServiceProvider)provider, ipAddress) ;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            stopwatch.Stop();
            
            bool isError = response == null || string.IsNullOrWhiteSpace(response);
            long responseTime = stopwatch.ElapsedMilliseconds;
            
            _apiVitalsLogic.RecordResponse((GeoLocationServiceProvider)provider, responseTime, isError);

            if(isError)
            {
                throw new Exception("Error in api call");
            }
            
            var responseDTO = JsonConvert.DeserializeObject<GeoLocationVendorResponseDTO>(response);
            if(responseDTO == null)
            {
                throw new Exception("Could not deserialize response");
            }
            GeoLocationBrokerResponseDTO result = new(ipAddress, responseDTO.CountryName, responseDTO.CityName);
            result.IsValid = true;
            return result;
        }

        private string GetProviderRequest(GeoLocationServiceProvider provider, string ipAddress)
        {
            switch(provider)
            {
                case GeoLocationServiceProvider.VENDOR_ONE:
                    return _config["ApiSettings:BaseUrl"] + _config["ApiSettings:VendorOneAddress"] + $"?ipAddress={ipAddress}";
                case GeoLocationServiceProvider.VENDOR_TWO:
                    return _config["ApiSettings:BaseUrl"] + _config["ApiSettings:VendorTwoAddress"] + $"?ipAddress={ipAddress}";
                case GeoLocationServiceProvider.VENDOR_THREE:
                    return _config["ApiSettings:BaseUrl"] + _config["ApiSettings:VendorThreeAddress"] + $"?ipAddress={ipAddress}";
                default:
                    return string.Empty;
            }
        }
    }
}