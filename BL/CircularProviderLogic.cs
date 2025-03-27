using ApiBroker.BL.Interfaces;
using ApiBroker.Entities.Enum;

namespace ApiBroker.BL
{
    public class CircularProviderSelector : ICircularProviderSelector
    {
        private readonly List<GeoLocationServiceProvider> _providers;
        private int _pointer = 0;
        private readonly object _lock = new();
        private readonly IApiVitalsLogic _apiVitalsLogic;

        public CircularProviderSelector(IApiVitalsLogic apiVitalsLogic)
        {
            _apiVitalsLogic = apiVitalsLogic;
            _providers = new()
            {
                GeoLocationServiceProvider.VENDOR_ONE,
                GeoLocationServiceProvider.VENDOR_TWO,
                GeoLocationServiceProvider.VENDOR_THREE
            };
        }

        public GeoLocationServiceProvider? GetProvider()
        {
            lock (_lock)
            {
                int start = _pointer;

                // Pass 1: Look for GREEN
                for (int i = 0; i < _providers.Count; i++)
                {
                    var index = (_pointer + i) % _providers.Count;
                    var candidate = _providers[index];

                    if (IsGeoLocationProvicerAvailable(candidate, ApiVitalsState.GREEN))
                    {
                        _pointer = (index + 1) % _providers.Count;
                        return candidate;
                    }
                }

                // Pass 2: Look for ORANGE
                for (int i = 0; i < _providers.Count; i++)
                {
                    var index = (_pointer + i) % _providers.Count;
                    var candidate = _providers[index];

                    if (IsGeoLocationProvicerAvailable(candidate, ApiVitalsState.ORANGE))
                    {
                        _pointer = (index + 1) % _providers.Count;
                        return candidate;
                    }
                }

                return null;
            }
        }

        private bool IsGeoLocationProvicerAvailable(GeoLocationServiceProvider candidate, ApiVitalsState acceptableState)
        {
            var state = _apiVitalsLogic.GetApiVitalsStateValue(candidate);
            return state != null && state.ApiVitalsState == acceptableState;
        }
    }
}
