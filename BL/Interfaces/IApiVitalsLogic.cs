using ApiBroker.Entities;
using ApiBroker.Entities.Enum;

namespace ApiBroker.BL.Interfaces
{
    public interface IApiVitalsLogic
    {
        public ServiceProviderMetrics GetApiVitalsStateValue(GeoLocationServiceProvider key);
        public void RecordResponse(GeoLocationServiceProvider provider, long responseTimeMs, bool isError);
    }
}