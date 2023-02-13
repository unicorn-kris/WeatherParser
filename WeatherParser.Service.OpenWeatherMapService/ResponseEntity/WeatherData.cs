namespace WeatherParser.Service.OpenWeatherMapService.ResponseEntity
{
    public class WeatherData
    {
        public int Dt { get; set; }
        public MainInfo Main { get; set; }
        public List<Weather> Weather { get; set; }
        public Cloud Clouds { get; set; }
        public Wind Wind { get; set; }
        public int Visibility { get; set; }
        public double Pop { get; set; }
        public Rain Rain { get; set; }
        public SysInfo Sys { get; set; }
        public string Dt_txt { get; set; }
    }
}
