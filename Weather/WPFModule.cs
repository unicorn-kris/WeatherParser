using Autofac;
using WeatherParser.Service;
using WeatherParser.WPF.ViewModels;
using TimerSaveDataService;

namespace WeatherParser.WPF
{
    public class WPFModule : Module
    {
        protected override void Load(Autofac.ContainerBuilder builder)
        {
            builder.RegisterModule<ServiceModule>();

            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();

            builder.RegisterModule<TimerSaveDataModule>();
        }
    }
}
