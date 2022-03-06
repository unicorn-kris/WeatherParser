using Autofac;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {
        private Autofac.IContainer _container;
       
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
