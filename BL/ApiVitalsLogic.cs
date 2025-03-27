using System.Collections.Concurrent;
using ApiBroker.BL.Interfaces;
using ApiBroker.Entities;
using ApiBroker.Entities.Enum;

public class ApiVitalsLogic : IApiVitalsLogic
{
    private readonly ConcurrentDictionary<GeoLocationServiceProvider, ServiceProviderMetrics> _apiVitalsData;
    private readonly ServiceProviderMetrics InitialServiceProviderMetrics = new(){
        ApiVitalsState = ApiVitalsState.GREEN
    };

    public ApiVitalsLogic()
    {
        _apiVitalsData = new();
        _apiVitalsData[GeoLocationServiceProvider.VENDOR_ONE] = InitialServiceProviderMetrics;
        _apiVitalsData[GeoLocationServiceProvider.VENDOR_TWO] = InitialServiceProviderMetrics;
        _apiVitalsData[GeoLocationServiceProvider.VENDOR_THREE] = InitialServiceProviderMetrics;
    }

    public Dictionary<GeoLocationServiceProvider, ServiceProviderMetrics> GetAllApiVitalsStateValues()
    {
        return _apiVitalsData.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    public ServiceProviderMetrics GetApiVitalsStateValue(GeoLocationServiceProvider key)
    {
        return _apiVitalsData.TryGetValue(key, out var value) ? value : new();
    }

    public void SetApiVitalsStateValue(GeoLocationServiceProvider key, ServiceProviderMetrics value)
    {
        _apiVitalsData[key] = value;
    }

    public void RecordResponse(GeoLocationServiceProvider provider, long responseTimeMs, bool isError)
    {
        var now = DateTime.UtcNow;
        var metrics = _apiVitalsData.GetOrAdd(provider, new ServiceProviderMetrics());

        metrics.ResponseTimeRecords.Enqueue((now, responseTimeMs));
        if (isError)
        {
            metrics.ErrorTimestamps.Enqueue(now);
        }

        while (metrics.ResponseTimeRecords.TryPeek(out var responseTimeRecord) && now - responseTimeRecord.Timestamp > TimeSpan.FromMinutes(5))
        {
            metrics.ResponseTimeRecords.TryDequeue(out _);
        }
        while (metrics.ErrorTimestamps.TryPeek(out var errorTimestamps) && now - errorTimestamps > TimeSpan.FromMinutes(5))
        {
            metrics.ErrorTimestamps.TryDequeue(out _);
        }
            
        if (metrics.ResponseTimeRecords.Count > 0)
        {
            metrics.ServiceProviderVitals.AvgResponseTime = (float)metrics.ResponseTimeRecords.Average(responseTimeRecord => responseTimeRecord.ResponseTimeMs);
            metrics.RequestsLastMinute = metrics.ResponseTimeRecords.Count(responseTimeRecord => now - responseTimeRecord.Timestamp <= TimeSpan.FromMinutes(1));
        }
        int totalRequests = metrics.ResponseTimeRecords.Count;
        int totalErrors = metrics.ErrorTimestamps.Count;
        metrics.ServiceProviderVitals.AvgErrorRate = totalRequests > 0 ? (float)totalErrors / totalRequests : 0f;

        // Update state based on thresholds
        metrics.ApiVitalsState = CalculateVitalsState(metrics);
    }

    private ApiVitalsState CalculateVitalsState(ServiceProviderMetrics metrics)
    {
        // rate limit reached
        if(metrics.RequestsLastMinute >= 10) // TODO :- use CONSTANT value here
        {
            return ApiVitalsState.RED;
        }
        // too many errors
        if (metrics.ServiceProviderVitals.AvgErrorRate >= 1f)
        {
            return ApiVitalsState.RED;
        }
        // callable but errors too high to be first consideration
        if (metrics.ServiceProviderVitals.AvgErrorRate >= 0.3f)
        {
            return ApiVitalsState.ORANGE;
        }
        // callable but response time is too high
        if (metrics.ServiceProviderVitals.AvgResponseTime > 500)
        {
            return ApiVitalsState.ORANGE;
        }
        return ApiVitalsState.GREEN;
    }
}
