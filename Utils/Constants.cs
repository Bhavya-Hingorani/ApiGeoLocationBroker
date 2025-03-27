namespace ApiBroker.Utils
{
    public static class Constants
    {
        public static readonly int MAX_ATTEMPS = 3;
        public static readonly int ERROR_RATE_TIME_SPAN = 5;
        public static readonly int LATENCY_TIME_SPAN = 5;
        public static readonly int MOCK_RATE_LIMIT = 10;
        public static readonly int MOCK_RATE_LIMIT_TIME_INTERVAL = 3;
        public static readonly int SERVICE_PROVIDER_COOLDOWN_TIME = 3;
        public static readonly float AVG_ERROR_RATE_HARD_THRESHOLD = 0.5f;
        public static readonly float AVG_ERROR_RATE_SOFT_THRESHOLD = 0.3f;
        public static readonly float AVG_LATENCY_THRESHOLD = 500;
    }
}