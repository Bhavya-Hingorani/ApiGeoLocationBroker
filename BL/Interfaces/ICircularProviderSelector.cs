using ApiBroker.DTOs;
using ApiBroker.Entities;
using ApiBroker.Entities.Enum;

namespace ApiBroker.BL.Interfaces
{
    public interface ICircularProviderSelector
    {
        public GeoLocationServiceProvider? GetProvider();
    }
}