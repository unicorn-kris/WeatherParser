namespace WeatherParser.Service.OpenWeatherMapService.ResponseEntity
{
    public class OpenWeatherResponse
    {
        public string Cod { get; set; }
        public int Message { get; set; }
        public int Cnt { get; set; }
        public List<WeatherData> List { get; set; }
        public City City { get; set; }
    }
}
