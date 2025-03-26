using ApiBroker.BL.Interfaces;

namespace ApiBroker.BL
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientWrapper> _logger;

        public HttpClientWrapper(HttpClient httpClient, ILogger<HttpClientWrapper> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetAsync(string requestLink)
        {
            try
            {
                var response = await _httpClient.GetAsync(requestLink);

                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                _logger.LogWarning("GET request failed: {StatusCode}", response?.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during GET request to {RequestLink}", requestLink);
            }

            return string.Empty;
        }
    }
}
