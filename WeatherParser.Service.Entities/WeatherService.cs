namespace WeatherParser.Service.Entities
{
    public class WeatherService
    {
        //объект для сохранения данных о погоде
        public DateTime Date { get; set; }
        public List<int> Hours { get; set; }
        public List<double> Temperature { get; set; }
        public List<double> Pressure { get; set; }
        public List<double> Humidity { get; set; }
        public List<double> WindSpeed { get; set; }
        public List<string> WindDirection { get; set; }

    }
}
