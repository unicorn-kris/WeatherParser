namespace WeatherParser.Service.OpenWeatherMapService.ResponseEntity
{
    public class MainInfo
    {
        public double Ttemp { get; set; }
        public double Feels_like { get; set; }
        public double Ttemp_min { get; set; }
        public double Temp_max { get; set; }
        public int Pressure { get; set; }
        public int Sea_level { get; set; }
        public int Grnd_level { get; set; }
        public int Humidity { get; set; }
        public double Temp_kf { get; set; }
    }
}
