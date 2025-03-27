using Newtonsoft.Json;

namespace ApiBroker.DTOs
{
    public class GeoLocationBrokerResponseDTO
    {
        public GeoLocationBrokerResponseDTO(string ipAddress, string country, string city)
        {
            IsValid = false;
            GeoLocationVendorResponseDTO = new GeoLocationVendorResponseDTO(ipAddress, country, city);
        }

        public GeoLocationBrokerResponseDTO(string ipAddress, string country, string city, string msg)
        {
            IsValid = false;
            ResponseMessage = msg;
            GeoLocationVendorResponseDTO = new GeoLocationVendorResponseDTO(ipAddress, country, city);
        }

        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
        [JsonProperty("responseMessage")]
        public string ResponseMessage { get; set; }
        [JsonProperty("geoLocationVendorResponse")]
        public GeoLocationVendorResponseDTO GeoLocationVendorResponseDTO { get; set; }
    }
}