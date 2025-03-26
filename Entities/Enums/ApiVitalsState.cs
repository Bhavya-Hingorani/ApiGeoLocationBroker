namespace ApiBroker.Entities.Enum 
{
    /*
        API can be one of the 3 states mentioned below
        RED - Rate limit reached and we absolutely cannot call the api
        ORANGE - Latency is too high, Response time too high
        GREEN - All is good and we can call it normally
    */
    public enum ApiVitalsState
    {
        RED = 0,
        ORANGE = 1,
        GREEN = 2,
    }
}