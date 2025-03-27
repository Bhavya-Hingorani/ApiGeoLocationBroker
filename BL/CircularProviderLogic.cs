using ApiBroker.BL.Interfaces;
using ApiBroker.Entities.Enum;
using ApiBroker.Utils;

namespace ApiBroker.BL
{
    /* Logic for picking the right provider ------------> */

    /* If all Providers are GREEN 
        
        if all providers are green that means they all are within acceptable latency and error rate
        this means that we can pick any one of them
        so here I have decided to cycle to a different provider on every new call
        this ensures that no one provider reaches their rate limit
        the above ensures that if another provider goes bad then the other 2 will still be GREEN
    */

    /* If the current provider is not green 
        
        When cycling through providers if one of them is not green we simply move to the next one
        this is because we might have another that is GREEN
        and is within the acceptable limits
    */

    /* If all Providers are ORANGE
    
        This means that every provider has some issue with either latency or error rate
        they are all still callable but we need to call the one that will give us no error in least amount of time
        We will simply pick the one that is fastest
        This ensures that even if a error is thrown we can quickly call another
        The intuition is that the above logic will work better than just cycling through them like we did when they where GREEN
    */

    /* If all Providers are RED
    
        This means that all providers are in bad state
        but that could mean either the error rate is too high or we have reached rate limit
        we obviously cant call the ones where rate limit is reached
        so we call the one that has not reached rate limit and is fastest
    */

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

        public GeoLocationServiceProvider? GetProvider(HashSet<GeoLocationServiceProvider>? exclude = null)
        {
            int start;
            lock (_lock)
            {
                start = _pointer;
            }

            // Pass 1: Use circular GREEN selection
            for (int i = 0; i < _providers.Count; i++)
            {
                var index = (start + i) % _providers.Count;
                var candidate = _providers[index];

                if (exclude != null && exclude.Contains(candidate)) continue;

                if (IsGeoLocationProviderAvailable(candidate, ApiVitalsState.GREEN))
                {
                    lock (_lock)
                    {
                        _pointer = (index + 1) % _providers.Count;
                    }
                    return candidate;
                }
            }

            // Pass 2: Pick lowest latency ORANGE (excluding failed)
            var orangeCandidates = _providers
                .Where(p =>
                {
                    if (exclude != null && exclude.Contains(p)) return false;
                    var state = _apiVitalsLogic.GetApiVitalsStateValue(p);
                    return state?.ApiVitalsState == ApiVitalsState.ORANGE;
                })
                .OrderBy(p => _apiVitalsLogic.GetApiVitalsStateValue(p)?.ServiceProviderVitals.AvgResponseTime ?? float.MaxValue)
                .ToList();

            if(orangeCandidates.Count > 0)
            {
                return orangeCandidates.FirstOrDefault();
            }

            // Pass 3: Pick lowest latency RED (excluding failed and rate limit reached)
            var redCandidates = _providers
                .Where(p =>
                {
                    if (exclude != null && exclude.Contains(p)) return false;
                    var state = _apiVitalsLogic.GetApiVitalsStateValue(p);
                    return state?.RequestsLastMinute < Constants.MOCK_RATE_LIMIT;
                })
                .OrderBy(p => _apiVitalsLogic.GetApiVitalsStateValue(p)?.ServiceProviderVitals.AvgResponseTime ?? float.MaxValue)
                .ToList();
            
            return redCandidates.Count > 0 ? redCandidates.FirstOrDefault() : GeoLocationServiceProvider.INVALID_VENDOR;
        }

        private bool IsGeoLocationProviderAvailable(GeoLocationServiceProvider candidate, ApiVitalsState acceptableState)
        {
            var state = _apiVitalsLogic.GetApiVitalsStateValue(candidate);
            return state != null && state.ApiVitalsState == acceptableState;
        }
    }
}
