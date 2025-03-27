using System.Collections.Concurrent;
using ApiBroker.BL.Interfaces;
using ApiBroker.Entities;
using ApiBroker.Entities.Enum;
using ApiBroker.Utils;

namespace ApiBroker.BL
{
    public class ApiVitalsLogic : IApiVitalsLogic
    {
        private readonly ConcurrentDictionary<GeoLocationServiceProvider, ServiceProviderMetrics> _apiVitalsData;
        private readonly ConcurrentDictionary<GeoLocationServiceProvider, bool> _cooldownTracker;

        public ApiVitalsLogic()
        {
            _apiVitalsData = new();
            _cooldownTracker = new();
            _apiVitalsData[GeoLocationServiceProvider.VENDOR_ONE] = GetInitialMetrics();
            _apiVitalsData[GeoLocationServiceProvider.VENDOR_TWO] = GetInitialMetrics();
            _apiVitalsData[GeoLocationServiceProvider.VENDOR_THREE] = GetInitialMetrics();
        }

        private ServiceProviderMetrics GetInitialMetrics()
        {
            return new(){
                ApiVitalsState = ApiVitalsState.GREEN
            };
        }

        public ServiceProviderMetrics GetApiVitalsStateValue(GeoLocationServiceProvider key)
        {
            return _apiVitalsData.TryGetValue(key, out var value) ? value : new();
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

            while (metrics.ResponseTimeRecords.TryPeek(out var responseTimeRecord) && now - responseTimeRecord.Timestamp > TimeSpan.FromMinutes(Constants.LATENCY_TIME_SPAN))
            {
                metrics.ResponseTimeRecords.TryDequeue(out _);
            }
            while (metrics.ErrorTimestamps.TryPeek(out var errorTimestamps) && now - errorTimestamps > TimeSpan.FromMinutes(Constants.ERROR_RATE_TIME_SPAN))
            {
                metrics.ErrorTimestamps.TryDequeue(out _);
            }
                
            if (metrics.ResponseTimeRecords.Count > 0)
            {
                metrics.ServiceProviderVitals.AvgResponseTime = (float)metrics.ResponseTimeRecords.Average(responseTimeRecord => responseTimeRecord.ResponseTimeMs);
                metrics.RequestsLastMinute = metrics.ResponseTimeRecords.Count(responseTimeRecord => now - responseTimeRecord.Timestamp <= TimeSpan.FromMinutes(Constants.MOCK_RATE_LIMIT_TIME_INTERVAL));
            }
            int totalRequests = metrics.ResponseTimeRecords.Count;
            int totalErrors = metrics.ErrorTimestamps.Count;
            metrics.ServiceProviderVitals.AvgErrorRate = totalRequests > 0 ? (float)totalErrors / totalRequests : 0f;

            // Update state based on thresholds
            metrics.ApiVitalsState = CalculateVitalsState(provider, metrics);
        }

        private ApiVitalsState CalculateVitalsState(GeoLocationServiceProvider provider, ServiceProviderMetrics metrics)
        {
            // rate limit reached
            if(metrics.RequestsLastMinute >= Constants.MOCK_RATE_LIMIT)
            {
                ScheduleCooldown(provider, TimeSpan.FromMinutes(Constants.MOCK_RATE_LIMIT_TIME_INTERVAL));
                return ApiVitalsState.RED;
            }
            // too many errors
            if (metrics.ServiceProviderVitals.AvgErrorRate >= Constants.AVG_ERROR_RATE_HARD_THRESHOLD)
            {
                ScheduleCooldown(provider, TimeSpan.FromMinutes(Constants.SERVICE_PROVIDER_COOLDOWN_TIME));
                return ApiVitalsState.RED;
            }
            // callable but errors too high to be first consideration
            if (metrics.ServiceProviderVitals.AvgErrorRate >= Constants.AVG_ERROR_RATE_SOFT_THRESHOLD)
            {
                ScheduleCooldown(provider, TimeSpan.FromMinutes(Constants.SERVICE_PROVIDER_COOLDOWN_TIME));
                return ApiVitalsState.ORANGE;
            }
            // callable but response time is too high
            if (metrics.ServiceProviderVitals.AvgResponseTime >= Constants.AVG_LATENCY_THRESHOLD)
            {
                ScheduleCooldown(provider, TimeSpan.FromMinutes(Constants.SERVICE_PROVIDER_COOLDOWN_TIME));
                return ApiVitalsState.ORANGE;
            }
            return ApiVitalsState.GREEN;
        }

        private void ScheduleCooldown(GeoLocationServiceProvider provider, TimeSpan cooldownDuration)
        {
            if (_cooldownTracker.ContainsKey(provider) && _cooldownTracker[provider])
            {
                return; // Cooldown already running for this provider
            }
            _cooldownTracker[provider] = true;
            Task.Run(async () =>
            {
                await Task.Delay(cooldownDuration);
                if (_apiVitalsData.TryGetValue(provider, out var metrics))
                {
                    metrics.ApiVitalsState = ApiVitalsState.GREEN;
                }
                _cooldownTracker[provider] = false;
            });
        }
    }
}
