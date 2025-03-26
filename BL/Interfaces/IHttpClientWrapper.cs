namespace ApiBroker.BL.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<string> GetAsync(string requestLink);
    }
}
