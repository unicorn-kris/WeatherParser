using Autofac;
using WeatherParser.Service.Common;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class GismeteoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAngleSharpServiceGismeteo>().Keyed<IWeatherPlugin>("Gismeteo").As<IWeatherPlugin>().SingleInstance();
        }
    }
}
