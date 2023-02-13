using Newtonsoft.Json;

namespace WeatherParser.Service.OpenWeatherMapService.ResponseEntity
{
    public class Rain
    {
        [JsonProperty("3h")]
        public double _3h { get; set; }
    }
}
