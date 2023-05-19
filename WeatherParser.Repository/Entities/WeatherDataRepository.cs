using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WeatherParser.Repository.Entities
{
    public class WeatherDataRepository
    {
        public Guid SiteID { get; set; }

        //IN this date i get a weather
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TargetDate { get; set; }

        public List<WeatherRepository> Weather { get; set; }
    }
}
