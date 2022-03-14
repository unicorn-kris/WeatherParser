using Autofac;
using System.Windows;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IContainer _container;

        public MainWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterModule<WPFModule>();
            _container = builder.Build();

            DataContext = _container.Resolve<IMainWindowViewModel>();
        }
    }
}
