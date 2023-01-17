﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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

            var filter = Builders<SiteRepository>.Filter.Eq(site => site.ID, siteID);

            return collection.Find(filter).FirstOrDefault();
        }
        #endregion

        public (DateTime, DateTime) GetFirstAndLastDate(Guid siteId)
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
                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteDocument.Name))
                    {
                        throw new Exception("DB for this site has not yet been added to the application");
                    }
                    else
                    {
                        var collectionSite = db.GetCollection<WeatherDataRepository>(siteDocument.Name);

                        var sort = Builders<WeatherDataRepository>.Sort.Descending(data => data.TargetDate);

                        var documents = collectionSite.Find(new BsonDocument()).Sort(sort).ToList();

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

        public void SaveWeatherData(WeatherDataRepository weatherData)
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
                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteDocument.Name))
                    {
                        db.CreateCollection(siteDocument.Name);
                    }

                    var collectionSite = db.GetCollection<WeatherDataRepository>(siteDocument.Name);

                    collectionSite.InsertOne(weatherData);
                }
            }
        }

        public List<WeatherDataRepository> GetAllWeatherData(DateTime targetDate, Guid siteId)
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
                    if (!db.ListCollectionNames().ToListAsync().Result.Any(col => col == siteDocument.Name))
                    {
                        throw new Exception("DB for this site has not yet been added to the application");
                    }
                    else
                    {
                        var collectionSite = db.GetCollection<WeatherDataRepository>(siteDocument.Name);

                        var documents = collectionSite.Find(new BsonDocument()).ToList();

                        //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов
                        //проще и быстрее проводить работу с поиском и сравнением данных с помощью словаря, где ключ - дата сбора данных
                        Dictionary<DateTime, WeatherRepository> dataInFiles = new Dictionary<DateTime, WeatherRepository>();

                        WeatherRepository weatherData = new WeatherRepository();

                        foreach (var document in documents)
                        {
                            //targetDate - ON this date i need a weather
                            foreach (var weather in document.Weather)
                            {
                                if (weather.Date == targetDate.Date)
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
                                }
                                dataInFiles.Add(document.TargetDate, weatherData);
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

        public List<SiteRepository> GetSites()
        {
            var db = GetDatabase();

            List<SiteRepository> sites = new List<SiteRepository>();

            //read .xml with sites names
            XmlSerializer ser = new XmlSerializer(typeof(List<object>));
            var fileValues = new List<object>();

            using (var file = new FileStream("../Helpers/Sites.xml", FileMode.Open))
            {
                fileValues = (List<object>)ser.Deserialize(file);
            }

            //create collections for all sites and get if we haven't collections
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
            //if we have some collections but in process was added some sites
            else
            {
                sites = db.GetCollection<SiteRepository>(SitesCollectionName).Find(new BsonDocument()).ToList();

                // /2 because we have site and his guid value
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