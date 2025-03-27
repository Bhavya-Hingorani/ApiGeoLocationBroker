using Newtonsoft.Json;

namespace ApiBroker.DTOs
{
    public class GeoLocationVendorResponseDTO
    {
        public GeoLocationVendorResponseDTO(string ipAddress, string country, string city)
        {
            IpAddress = ipAddress;
            CountryName = country;
            CityName = city;
        }

        [JsonProperty("ip")]
        public string IpAddress {get; set;}

        [JsonProperty("countryName")]
        public string CountryName {get; set;}

        [JsonProperty("cityName")]
        public string CityName {get; set;}
    }
}