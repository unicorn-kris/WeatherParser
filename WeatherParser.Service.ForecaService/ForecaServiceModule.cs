using Autofac;
using WeatherParser.Service.Common;

namespace WeatherParser.Service.Plugins.ForecaService
{
    public class ForecaServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAngleSharpServiceForeca>().Keyed<IWeatherPlugin>("Foreca").As<IWeatherPlugin>().SingleInstance();
        }
    }
}
