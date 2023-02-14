using Autofac;
using MongoDB.Driver;
using WeatherParser.Repository;
using WeatherParser.Repository.Contract;
using WeatherParser.Service.Common;
using WeatherParser.Service.OpenWeatherMapService;
using WeatherParser.Service.Plugins.GismeteoService;

namespace WeatherParser.WinService
{
    public class Program
    {
        public static void Main()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(s =>
                {
                    var builder = new ContainerBuilder();

                    //register all plugins modules
                    s.AddTransient<IMongoClient>(с => new MongoClient("mongodb://localhost:27017"));
                    s.AddTransient<HttpClient>(с => new HttpClient());

                    s.AddTransient<IWeatherParserRepository, WeatherDataNoSQLRepository>();
                    s.AddTransient<IWeatherPlugin, WeatherDataAngleSharpServiceGismeteo>();
                    s.AddTransient<IWeatherPlugin, WeatherDataAPIServiceOpenWeatherMap>();

                    //s.AddTransient<RepositoryModule>();

                    s.AddHostedService<SaveWeatherWorker>();
                })
                .UseWindowsService()
                .Build();

            host.Run();
        }
    }
}
