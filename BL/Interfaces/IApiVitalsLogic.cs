using ApiBroker.DTOs;
using ApiBroker.Entities;
using ApiBroker.Entities.Enum;

namespace ApiBroker.BL.Interfaces
{
    public interface IApiVitalsLogic
    {
        public Dictionary<GeoLocationServiceProvider, ServiceProviderMetrics> GetAllApiVitalsStateValues();
        public ServiceProviderMetrics GetApiVitalsStateValue(GeoLocationServiceProvider key);
        public void SetApiVitalsStateValue(GeoLocationServiceProvider key, ServiceProviderMetrics value);
    }
}