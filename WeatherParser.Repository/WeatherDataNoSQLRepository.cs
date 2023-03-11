﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Common;

namespace WeatherParser.Repository
{
    public class WeatherDataNoSQLRepository : IWeatherParserRepository
    {
        #region Private
        private const string SitesCollectionName = "SitesCollection";

        private const string DBName = "WeatherDb";

        private readonly IMongoDatabase _db;

        public WeatherDataNoSQLRepository(IMongoClient dbClient)
        {
            _db = dbClient.GetDatabase(DBName);
        }

        private async Task<SiteRepository> GetSiteDocumentAsync(IMongoDatabase _db, Guid siteID)
        {
            var collection = _db.GetCollection<SiteRepository>(SitesCollectionName);

            var fieldsBuilder = Builders<SiteRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            var filter = Builders<SiteRepository>.Filter.Eq(site => site.ID, siteID);

            return await collection.Find(filter).Project<SiteRepository>(fields).FirstOrDefaultAsync();
        }

        private async Task<string> GetSiteCollectionNameAsync(Guid siteId)
        {
            var collectionNames = await _db.ListCollectionNames().ToListAsync();

            if (!collectionNames.Any(col => col == SitesCollectionName))
            {
                throw new Exception("Sites has not yet been added to the application");
            }

            var siteDocument = await GetSiteDocumentAsync(_db, siteId);

            if (siteDocument == null)
            {
                throw new Exception("This site has not yet been added to the application");
            }

            var siteCollectionName = siteDocument.Name + "Collection";

            if (!collectionNames.Any(col => col == siteCollectionName))
            {
                return null;
            }

            return siteCollectionName;
        }
        #endregion

        public async Task<(DateTime, DateTime)> GetFirstAndLastDateAsync(Guid siteId)
        {
            var siteCollectionName = await GetSiteCollectionNameAsync(siteId);

            if (siteCollectionName == null)
            {
                throw new Exception("This site has not yet been added to the application");
            }

            var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            var collectionSite = _db.GetCollection<WeatherDataRepository>(siteCollectionName);

            var sortUp = Builders<WeatherDataRepository>.Sort.Ascending(data => data.TargetDate);
            var sortDown = Builders<WeatherDataRepository>.Sort.Descending(data => data.TargetDate);

            var firstDate = _db.GetCollection<WeatherDataRepository>(siteCollectionName)
                .Find(new BsonDocument())
                .Project<WeatherDataRepository>(fields)
                .Sort(sortUp)
                .FirstOrDefault()
                ;

            var lastDate = _db.GetCollection<WeatherDataRepository>(siteCollectionName)
                .Find(new BsonDocument())
                .Project<WeatherDataRepository>(fields)
                .Sort(sortDown)
                .FirstOrDefault()
                ;

            if (firstDate != null && lastDate != null)
            {
                firstDate.Weather.Sort((x, y) => x.Date.CompareTo(y.Date));
                lastDate.Weather.Sort((x, y) => x.Date.CompareTo(y.Date));

                return (firstDate.Weather.First().Date, lastDate.Weather.Last().Date);
            }
            else
            {
                throw new Exception("Data for this site has not yet been added to the application");
            }
        }

        public async Task SaveWeatherDataAsync(WeatherDataRepository weatherData)
        {
            var siteCollectionName = await GetSiteCollectionNameAsync(weatherData.SiteID);

            if (siteCollectionName == null)
            {
                throw new Exception("This site has not yet been added to the application");
            }

            var collectionSite = _db.GetCollection<WeatherDataRepository>(siteCollectionName);

            var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            if (_db.GetCollection<WeatherDataRepository>(siteCollectionName).Find(w => w.TargetDate.Date.Equals(weatherData.TargetDate.Date)).Project<WeatherDataRepository>(fields) == null)
            {
                await collectionSite.InsertOneAsync(weatherData);
            }
        }

        public async Task<List<WeatherDataRepository>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId)
        {
            var siteCollectionName = await GetSiteCollectionNameAsync(siteId);

            if (siteCollectionName == null)
            {
                throw new Exception("This site has not yet been added to the application");
            }

            var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            //datetime have no locality and we need to have this parameter in utc for find date, because expression in "find" will go to database, where data saved in utc => datetime.pase(datetime.tostring())
            var documents = await _db.GetCollection<WeatherDataRepository>(siteCollectionName).Find(w => w.Weather.Any(wd => wd.Date.Equals(DateTime.Parse(targetDate.ToString())))).Project<WeatherDataRepository>(fields).ToListAsync();
            
            //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов
            //проще и быстрее проводить работу с поиском и сравнением данных с помощью словаря, где ключ - дата сбора данных
            Dictionary<DateTime, WeatherRepository> dataInFiles = new Dictionary<DateTime, WeatherRepository>();

            foreach (var document in documents)
            {
                //targetDate - ON this date i need a weather
                foreach (var weather in document.Weather)
                {
                    if (weather.Date.Date == targetDate.Date.Date)
                    {
                        var weatherData = new WeatherRepository()
                        {
                            Hours = new List<int>(),
                            Temperature = new List<double>(),
                            Humidity = new List<double>(),
                            Pressure = new List<double>(),
                            WindDirection = new List<string>(),
                            WindSpeed = new List<double>()
                        };

                        weatherData.Date = weather.Date;

                        weatherData.Temperature = weather.Temperature;

                        weatherData.Humidity = weather.Humidity;

                        weatherData.Pressure = weather.Pressure;

                        weatherData.WindDirection = weather.WindDirection;

                        weatherData.WindSpeed = weather.WindSpeed;

                        weatherData.Hours = weather.Hours;

                        if (!dataInFiles.ContainsKey(document.TargetDate.Date))
                        {
                            dataInFiles.Add(document.TargetDate.Date, weatherData);
                        }
                    }
                }
            }

            List<WeatherDataRepository> resultData = new List<WeatherDataRepository>();

            foreach (var weather in dataInFiles)
            {
                resultData.Add(new WeatherDataRepository() { TargetDate = weather.Key, Weather = new List<WeatherRepository>() { weather.Value } });
            }

            return resultData;
        }

        public async Task<List<WeatherDataRepository>> GetAllWeatherDataBySiteAsync(Guid siteId)
        {
            var siteCollectionName = await GetSiteCollectionNameAsync(siteId);

            if (siteCollectionName == null)
            {
                throw new Exception("This site has not yet been added to the application");
            }

            var fieldsBuilder = Builders<WeatherDataRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            var documents = await _db.GetCollection<WeatherDataRepository>(siteCollectionName).Find(new BsonDocument()).Project<WeatherDataRepository>(fields).ToListAsync();

            //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов
            //проще и быстрее проводить работу с поиском и сравнением данных с помощью словаря, где ключ - дата сбора данных
            Dictionary<DateTime, WeatherRepository> dataInFiles = new Dictionary<DateTime, WeatherRepository>();

            foreach (var document in documents)
            {
                //targetDate - ON this date i need a weather
                foreach (var weather in document.Weather)
                {
                    var weatherData = new WeatherRepository()
                    {
                        Hours = new List<int>(),
                        Temperature = new List<double>(),
                        Humidity = new List<double>(),
                        Pressure = new List<double>(),
                        WindDirection = new List<string>(),
                        WindSpeed = new List<double>()
                    };

                    weatherData.Date = weather.Date;

                    weatherData.Temperature = weather.Temperature;

                    weatherData.Humidity = weather.Humidity;

                    weatherData.Pressure = weather.Pressure;

                    weatherData.WindDirection = weather.WindDirection;

                    weatherData.WindSpeed = weather.WindSpeed;

                    weatherData.Hours = weather.Hours;

                    if (!dataInFiles.ContainsKey(document.TargetDate.Date))
                    {
                        dataInFiles.Add(document.TargetDate.Date, weatherData);
                    }
                }
            }

            List<WeatherDataRepository> resultData = new List<WeatherDataRepository>();

            foreach (var weather in dataInFiles)
            {
                resultData.Add(new WeatherDataRepository() { TargetDate = weather.Key, Weather = new List<WeatherRepository>() { weather.Value } });
            }

            return resultData;
        }

        public async Task<List<SiteRepository>> GetSitesAsync()
        {
            var sites = new List<SiteRepository>();

            var fieldsBuilder = Builders<SiteRepository>.Projection;
            var fields = fieldsBuilder.Exclude("_id");

            var DBsites = _db.GetCollection<SiteRepository>(SitesCollectionName).Find(new BsonDocument()).Project<SiteRepository>(fields).ToList();

            foreach (var DBsite in DBsites)
            {
                sites.Add(new SiteRepository() { ID = DBsite.ID, Name = DBsite.Name, Rating = DBsite.Rating });
            }

            return sites;
        }

        public async Task AddSitesAsync(IEnumerable<IWeatherPlugin> plugins)
        {
            var fileValues = new List<SiteRepository>();

            foreach (var plugin in plugins)
            {
                fileValues.Add(new SiteRepository() { ID = plugin.SiteID, Name = plugin.Name });
            }

            //create collections for all sites and get if we haven't collections
            var collectionNames = await _db.ListCollectionNames().ToListAsync();

            if (!collectionNames.Any(col => col == SitesCollectionName))
            {
                _db.CreateCollection(SitesCollectionName);

                var sitesCollection = _db.GetCollection<SiteRepository>(SitesCollectionName);

                await sitesCollection.InsertManyAsync(fileValues);
            }
            //if we have some collections but in process was added some sites
            else
            {
                var fieldsBuilder = Builders<SiteRepository>.Projection;
                var fields = fieldsBuilder.Exclude("_id");

                var DBsites = _db.GetCollection<SiteRepository>(SitesCollectionName).Find(new BsonDocument()).Project<SiteRepository>(fields).ToList();

                var sitesCollection = _db.GetCollection<SiteRepository>(SitesCollectionName);

                foreach (var site in fileValues)
                {
                    if (sitesCollection.Find(Builders<SiteRepository>.Filter.Eq(siteFromCol => siteFromCol.ID, site.ID)).Project<SiteRepository>(fields).FirstOrDefault() == null)
                    {
                        await sitesCollection.InsertOneAsync(site);
                    }
                    var siteCollectionName = site.Name + "Collection";

                    if (!collectionNames.Any(col => col == siteCollectionName))
                    {
                        _db.CreateCollection(siteCollectionName);
                    }
                }
            }
        }

    }
}
