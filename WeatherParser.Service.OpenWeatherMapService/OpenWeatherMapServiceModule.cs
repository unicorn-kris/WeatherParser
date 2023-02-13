using Autofac;
using WeatherParser.Service.Common;

namespace WeatherParser.Service.OpenWeatherMapService
{
    public class OpenWeatherMapServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAPIServiceOpenWeatherMap>().Keyed<IWeatherPlugin>("OpenWeatherMap").As<IWeatherPlugin>().SingleInstance();

            builder.RegisterType<HttpClient>().AsSelf().SingleInstance();
        }
    }
}
