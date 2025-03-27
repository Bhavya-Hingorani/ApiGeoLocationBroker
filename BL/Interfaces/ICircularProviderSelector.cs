using ApiBroker.Entities.Enum;

namespace ApiBroker.BL.Interfaces
{
    public interface ICircularProviderSelector
    {
        public GeoLocationServiceProvider? GetProvider(HashSet<GeoLocationServiceProvider>? exclude);
    }
}