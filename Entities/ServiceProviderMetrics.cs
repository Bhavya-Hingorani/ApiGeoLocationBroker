using System.Collections.Concurrent;
using ApiBroker.Entities.Enum;

namespace ApiBroker.Entities
{
    public class ServiceProviderMetrics
    {
        public ServiceProviderMetrics(){
            ApiVitalsState = ApiVitalsState.RED;
            ServiceProviderVitals = new();
            RequestsLastMinute = int.MaxValue;

            ResponseTimeRecords = new ConcurrentQueue<(DateTime Timestamp, long ResponseTimeMs)>();
            ErrorTimestamps = new ConcurrentQueue<DateTime>();
        }
        public ApiVitalsState ApiVitalsState { get; set; }
        public ServiceProviderVitals ServiceProviderVitals { get; set; }
        public int RequestsLastMinute {get; set;} 

        public ConcurrentQueue<(DateTime Timestamp, long ResponseTimeMs)> ResponseTimeRecords { get; set; }
        public ConcurrentQueue<DateTime> ErrorTimestamps { get; set; }
    }
}