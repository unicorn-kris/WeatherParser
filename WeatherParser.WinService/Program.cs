namespace WeatherParser.WinService
{
    public class Program
    {
        public static void Main()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(s =>
                {
                    s.AddHostedService<SaveWeatherWorker>();
                })
                .Build();
        }
    }
}
