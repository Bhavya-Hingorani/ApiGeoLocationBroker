using ApiBroker.BL.Interfaces;

namespace ApiBroker.BL
{
    public class ApiBrokerLogic : IApiBrokerLogic
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public ApiBrokerLogic(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }
        
        public async Task<bool> GetGeoLocationLogic(string ipAddress)
        {
            string requestUrl = $"https://localhost:7185/api/vendor1/get/location/?ipAddress={ipAddress}";
            var response = await _httpClientWrapper.GetAsync(requestUrl);
            if(response == null || string.IsNullOrWhiteSpace(response))
            {
                return false;
            }
            return true;
        }
    }
}