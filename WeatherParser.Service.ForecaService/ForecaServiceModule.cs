using Autofac;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class ForecaServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAngleSharpServiceForeca>().As<IWeatherParserServiceForeca>();
        }
    }
}
