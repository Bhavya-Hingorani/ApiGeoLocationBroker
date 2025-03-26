using ApiBroker.DTOs;
using ApiBroker.Entities.Enum;

namespace ApiBroker.BL.Interfaces
{
    public interface IApiVitalsLogic
    {
        public ApiVitalsState GetApiVitalsStateValue(GeoLocationServiceProvider key);
        public void SetApiVitalsStateValue(GeoLocationServiceProvider key, ApiVitalsState value);
    }
}