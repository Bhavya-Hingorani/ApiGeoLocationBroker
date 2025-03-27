namespace ApiBroker.Utils
{
    public static class Validation
    {
        public static (bool, string) IsRequestValid(string ipAddress)
        {
            if(string.IsNullOrWhiteSpace(ipAddress))
            {
                return (false, "Request is null");
            }
            if(!System.Net.IPAddress.TryParse(ipAddress, out _))
            {
                return (false, $"Invalid IP Address format: {ipAddress}");
            }
            return (true, string.Empty);
        }
    }
}