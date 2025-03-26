using System.Collections.Concurrent;
using ApiBroker.BL.Interfaces;
using ApiBroker.Entities.Enum;

// TODO :- Add the functions that make setting the ApiVitalState dynamic (Latency, errorRate and calls made)
public class ApiVitalsLogic : IApiVitalsLogic
{
    private readonly ConcurrentDictionary<GeoLocationServiceProvider, ApiVitalsState> _apiVitalsData;

    public ApiVitalsLogic()
    {
        _apiVitalsData = new();
    }

    public ApiVitalsState GetApiVitalsStateValue(GeoLocationServiceProvider key)
    {
        return _apiVitalsData.TryGetValue(key, out var value) ? value : ApiVitalsState.RED;
    }

    public void SetApiVitalsStateValue(GeoLocationServiceProvider key, ApiVitalsState value)
    {
        _apiVitalsData[key] = value;
    }
}
