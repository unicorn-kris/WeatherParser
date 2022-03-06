using Autofac;
using WeatherParser.Service;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF
{
    public class WPFModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ServiceModule>();

            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();
        }
    }
}
