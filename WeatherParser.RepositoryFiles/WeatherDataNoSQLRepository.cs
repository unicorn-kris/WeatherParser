using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;

namespace WeatherParser.RepositoryFiles
{
    public class WeatherDataNoSQLRepository : IWeatherParserRepository
    {
        private const string SitesCollectionName = "SitesCollection";
        public DateTime GetFirstDate()
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastDate()
        {
            throw new NotImplementedException();
        }

        public void SaveWeatherData(WeatherDataRepository weatherData)
        {
            throw new NotImplementedException();
        }

        public List<WeatherDataRepository> GetAllWeatherData(DateTime targetDate)
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");

            var db = dbClient.GetDatabase("WeatherDb");


            throw new NotImplementedException();

        }

        public List<SiteRepository> GetSites()
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");
            var db = dbClient.GetDatabase("WeatherDb");

            List<SiteRepository> sites = new List<SiteRepository>();

            //read .xml with sites names
            XmlSerializer ser = new XmlSerializer(typeof(List<object>));
            var fileValues = new List<object>();

            using (var file = new FileStream("../Helpers/Sites.xml", FileMode.Open))
            {
                fileValues = (List<object>)ser.Deserialize(file);
            }

            if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == SitesCollectionName))
            {
                db.CreateCollection(SitesCollectionName);

                for (int i = 0; i < fileValues.Count - 2; i += 2)
                {
                    sites.Add(new SiteRepository() { Name = (string)fileValues[i], ID = (Guid)fileValues[i + 1], Rating = default });
                }

                var sitesCollection = db.GetCollection<SiteRepository>(SitesCollectionName);

                sitesCollection.InsertMany(sites);
            }
            else
            {
                sites = db.GetCollection<SiteRepository>(SitesCollectionName).Find(new BsonDocument()).ToList();

                if (fileValues.Count / 2 > sites.Count)
                {
                    var sitesCollection = db.GetCollection<SiteRepository>(SitesCollectionName);

                    foreach (var site in sites)
                    {
                        if (sitesCollection.Find(Builders<SiteRepository>.Filter.Eq(siteFromCol => siteFromCol.ID, site.ID)) == null)
                        {
                            sitesCollection.InsertOne(site);
                        }
                    }
                }
            }

            return sites;
        }
    }
}
