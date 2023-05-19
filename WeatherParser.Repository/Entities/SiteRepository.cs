using System;

namespace WeatherParser.Repository.Entities
{
    public class SiteRepository
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public float Rating { get; set; }
    }
}
