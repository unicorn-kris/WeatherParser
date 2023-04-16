using Autofac;
using System.Windows;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF
{
    /// <summary>
    /// Interaction logic for ExcelLoadWindow.xaml
    /// </summary>
    public partial class ExcelLoadWindow : Window
    {
        private IContainer _container;

        public ExcelLoadWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterModule<WPFModule>();
            _container = builder.Build();

            DataContext = _container.Resolve<ExcelWindowViewModel>();
        }
    }
}
 