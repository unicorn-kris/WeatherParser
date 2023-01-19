using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository
{
    public class WeatherDataNoSQLRepository : IWeatherParserRepository
    {
        #region Private
        private const string SitesCollectionName = "SitesCollection";

        private IMongoDatabase GetDatabase()
        {
            var dbClient = new MongoClient("mongodb://localhost:27017");

            return dbClient.GetDatabase("WeatherDb");
        }

        private SiteRepository GetSiteDocument(IMongoDatabase db, Guid siteID)
        {
            var collection = db.GetCollection<SiteRepository>(SitesCollectionName);

            var fieldsBuilder = Builders<SiteRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            var filter = Builders<SiteRepository>.Filter.Eq(site => site.ID, siteID);

            return collection.Find(filter).Project<SiteRepository>(fields).FirstOrDefault();
        }
        #endregion

        public async Task<(DateTime, DateTime)> GetFirstAndLastDateAsync(Guid siteId)
        {
            var db = GetDatabase();

            if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == SitesCollectionName))
            {
                throw new Exception("Sites has not yet been added to the application");
            }
            else
            {
                var siteDocument = GetSiteDocument(db, siteId);

                if (siteDocument == null)
                {
                    throw new Exception("This site has not yet been added to the application");
                }
                else
                {
                    var siteCollectionName = siteDocument.Name + "Collection";

                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteCollectionName))
                    {
                        throw new Exception("DB for this site has not yet been added to the application");
                    }
                    else
                    {
                        var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
                        var fields = fieldsBuilder.Exclude("_id");

                        var collectionSite = db.GetCollection<WeatherDataRepository>(siteCollectionName);

                        var sort = Builders<WeatherDataRepository>.Sort.Descending(data => data.TargetDate);

                        var documents = await db.GetCollection<WeatherDataRepository>(siteCollectionName).Find(new BsonDocument()).Project<WeatherDataRepository>(fields).Sort(sort).ToListAsync();
                        
                        if (documents.Any())
                        {
                            documents.FirstOrDefault().Weather.Sort(delegate (WeatherRepository x, WeatherRepository y)
                            {
                                return x.Date.CompareTo(y.Date);
                            });

                            documents.LastOrDefault().Weather.Sort(delegate (WeatherRepository x, WeatherRepository y)
                            {
                                return x.Date.CompareTo(y.Date);
                            });


                            return (documents.First().Weather.First().Date, documents.Last().Weather.Last().Date);
                        }
                        else
                        {
                            throw new Exception("Data for this site has not yet been added to the application");
                        }
                    }
                }
            }
        }

        public async Task SaveWeatherDataAsync(WeatherDataRepository weatherData)
        {
            var db = GetDatabase();

            if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == SitesCollectionName))
            {
                throw new Exception("Sites has not yet been added to the application");
            }
            else
            {
                var siteDocument = GetSiteDocument(db, weatherData.SiteID);

                if (siteDocument == null)
                {
                    throw new Exception("This site has not yet been added to the application");
                }
                else
                {
                    var siteCollectionName = siteDocument.Name + "Collection";

                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteCollectionName))
                    {
                        await db.CreateCollectionAsync(siteCollectionName);
                    }

                    var collectionSite = db.GetCollection<WeatherDataRepository>(siteCollectionName);

                   await collectionSite.InsertOneAsync(weatherData);
                }
            }
        }

        public async Task<List<WeatherDataRepository>> GetAllWeatherDataAsync(DateTime targetDate, Guid siteId)
        {
            var db = GetDatabase();

            if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == SitesCollectionName))
            {
                throw new Exception("Sites has not yet been added to the application");
            }
            else
            {
                var siteDocument = GetSiteDocument(db, siteId);

                if (siteDocument == null)
                {
                    throw new Exception("This site has not yet been added to the application");
                }
                else
                {
                    var siteCollectionName = siteDocument.Name + "Collection";

                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteCollectionName))
                    {
                        throw new Exception("DB for this site has not yet been added to the application");
                    }
                    else
                    {

                        var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
                        var fields = fieldsBuilder.Exclude("_id");

                        var documents = await db.GetCollection<WeatherDataRepository>(siteCollectionName).Find(new BsonDocument()).Project<WeatherDataRepository>(fields).ToListAsync();


                        //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов
                        //проще и быстрее проводить работу с поиском и сравнением данных с помощью словаря, где ключ - дата сбора данных
                        Dictionary<DateTime, WeatherRepository> dataInFiles = new Dictionary<DateTime, WeatherRepository>();

                        WeatherRepository weatherData = new WeatherRepository();

                        foreach (var document in documents)
                        {
                            //targetDate - ON this date i need a weather
                            foreach (var weather in document.Weather)
                            {
                                if (weather.Date.Date == targetDate.Date.Date)
                                {
                                    weatherData = new WeatherRepository()
                                    {
                                        Temperature = new List<double>(),
                                        Humidity = new List<int>(),
                                        Pressure = new List<int>(),
                                        WindDirection = new List<string>(),
                                        WindSpeed = new List<int>()
                                    };

                                    weatherData.Date = weather.Date;

                                    weatherData.Temperature = weather.Temperature;

                                    weatherData.Humidity = weather.Humidity;

                                    weatherData.Pressure = weather.Pressure;

                                    weatherData.WindDirection = weather.WindDirection;

                                    weatherData.WindSpeed = weather.WindSpeed;

                                    if (!dataInFiles.ContainsKey(document.TargetDate.Date))
                                    {
                                        dataInFiles.Add(document.TargetDate.Date, weatherData);
                                    }
                                }
                            }
                        }


                        //преобразование словаря в возвращаемый тип
                        List<WeatherDataRepository> resultData = new List<WeatherDataRepository>();

                        foreach (var weather in dataInFiles)
                        {
                            resultData.Add(new WeatherDataRepository() { TargetDate = weather.Key, Weather = new List<WeatherRepository>() { weather.Value } });
                        }

                        return resultData;
                    }
                }
            }
        }

        public async Task<List<SiteRepository>> GetSitesAsync()
        {
            var db = GetDatabase();

            var sites = new List<SiteRepository>();

            //read .json with sites names
            var fileValues = new List<SiteRepository>();

            using (var file = new StreamReader("../Helpers/Sites.json"))
            {
                string json = file.ReadToEnd().Trim();
                foreach (var str in json.Split('\n'))
                {
                    fileValues.Add(JsonConvert.DeserializeObject<SiteRepository>(str));
                }
            }

            //create collections for all sites and get if we haven't collections
            if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == SitesCollectionName))
            {
                db.CreateCollection(SitesCollectionName);

                var sitesCollection = db.GetCollection<SiteRepository>(SitesCollectionName);

                await sitesCollection.InsertManyAsync(fileValues);

                foreach (var site in fileValues)
                {
                    sites.Add(site);
                }
            }
            //if we have some collections but in process was added some sites
            else
            {
                var fieldsBuilder = Builders<SiteRepository>.Projection;
                var fields = fieldsBuilder.Exclude("_id");

                var DBsites = db.GetCollection<SiteRepository>(SitesCollectionName).Find(new BsonDocument()).Project<SiteRepository>(fields).ToList();

                foreach (var DBsite in DBsites)
                {
                    sites.Add(new SiteRepository() { ID = DBsite.ID, Name = DBsite.Name, Rating = DBsite.Rating });
                }

                // /2 because we have site and his guid value
                if (fileValues.Count / 2 > sites.Count)
                {
                    var sitesCollection = db.GetCollection<SiteRepository>(SitesCollectionName);

                    foreach (var site in fileValues)
                    {
                        if (sitesCollection.Find(Builders<SiteRepository>.Filter.Eq(siteFromCol => siteFromCol.ID, site.ID)).FirstOrDefault() == null)
                        {
                            await sitesCollection.InsertOneAsync(site);
                            sites.Add(site);
                        }
                    }
                }
            }

            return sites;
        }
    }
}
