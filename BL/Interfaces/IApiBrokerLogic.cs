using ApiBroker.DTOs;

namespace ApiBroker.BL.Interfaces
{
    public interface IApiBrokerLogic
    {
        Task<GeoLocationBrokerResponseDTO> GetGeoLocationLogic(string ipAddress, int attemps);
    }
}