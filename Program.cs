
using ApiBroker.BL;
using ApiBroker.BL.Interfaces;

namespace ApiBroker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // doing dependency injection
            builder.Services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();
            builder.Services.AddScoped<IApiBrokerLogic, ApiBrokerLogic>();
            builder.Services.AddSingleton<IApiVitalsLogic, ApiVitalsLogic>();

            var app = builder.Build();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
