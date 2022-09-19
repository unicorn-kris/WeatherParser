namespace WeatherParser.Service.Entities
{
    public class WeatherDataService
    {
        public Guid SiteId { get; set; }
        public DateTime TargetDate { get; set; }
        public List<WeatherService> Weather { get; set; }
    }
}
