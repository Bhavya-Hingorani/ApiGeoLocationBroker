namespace ApiBroker.BL.Interfaces
{
    public interface IApiBrokerLogic
    {
        Task<bool> GetGeoLocationLogic(string ipAddress);
    }
}