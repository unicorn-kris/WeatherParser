using Autofac;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class GismeteoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAngleSharpServiceGismeteo>().As<IWeatherParserServiceGismeteo>();
        }
    }
}
