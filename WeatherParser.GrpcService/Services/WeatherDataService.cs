using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherParser.Service.Contract;

namespace WeatherParser.GrpcService.Services
{
    public class WeatherDataService : WeatherDataProtoGismeteo.WeatherDataProtoGismeteoBase
    {
        private readonly ILogger<IService> _logger;
        private readonly IService _weatherParserService;

        public WeatherDataService(ILogger<IService> logger, IService weatherParserService)
        {
            _logger = logger;
            _weatherParserService = weatherParserService;
        }

        public override async Task<WeatherDataGetResponse> GetAllWeatherDataByDay(WeatherDataRequest request, ServerCallContext context)
        {
            try
            {
                List<Service.Entities.WeatherDataService> weatherData = await _weatherParserService.GetAllWeatherDataByDayAsync(request.Date.ToDateTime(), new Guid(request.SiteID));

                return MapServiceEntity(weatherData);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<WeatherDataGetResponse> GetDeviationsOfRealFromForecast(WeatherDataRequest request, ServerCallContext context)
        {
            try
            {
                List<Service.Entities.WeatherDataService> weatherData = await _weatherParserService.GetDeviationsOfRealFromForecast(request.Date.ToDateTime(), new Guid(request.SiteID));

                return MapServiceEntity(weatherData);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetDeviationsOfRealFromForecast failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<WeatherDataGetResponse> GetMeanDeviationsOfRealForecast(GetMeanDeviationsRequest request, ServerCallContext context)
        {
            try
            {
                List<Service.Entities.WeatherDataService> weatherData = await _weatherParserService.GetMeanDeviationsOfRealForecast(new Guid(request.SiteID), request.Days);

                return MapServiceEntity(weatherData);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMeanDeviationsOfRealForecast failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<FirstLastDates> GetFirstAndLastDate(SiteID request, ServerCallContext context)
        {
            try
            {
                var dates = await _weatherParserService.GetFirstAndLastDateAsync(new Guid(request.ID));

                return new FirstLastDates()
                {
                    FirstDate = DateTime.SpecifyKind(dates.firstDate, DateTimeKind.Utc).ToTimestamp(),
                    LastDate = DateTime.SpecifyKind(dates.lastDate, DateTimeKind.Utc).ToTimestamp()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirstAndLastDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<SitesList> GetSites(Empty request, ServerCallContext context)
        {
            try
            {
                var sitesService = await _weatherParserService.GetSitesAsync();
                var resultSites = new SitesList();
                foreach (var site in sitesService)
                {
                    resultSites.Sites.Add(new Site() { SiteId = site.ID.ToString(), SiteName = site.Name });
                }
                return resultSites;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSites failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private WeatherDataGetResponse MapServiceEntity(List<Service.Entities.WeatherDataService> weatherData)
        {
            var returnWeatherData = new WeatherDataGetResponse();

            foreach (var newWeatherData in weatherData)
            {
                var newList = new WeatherDataList();

                foreach (var item in newWeatherData.Weather)
                {
                    var temps = new Temperatures();
                    foreach (var temp in item.Temperature)
                    {
                        temps.Temperature.Add(temp);
                    }

                    var hums = new Humidities();
                    foreach (var hum in item.Humidity)
                    {
                        hums.Humidity.Add(hum);
                    }

                    var press = new Pressures();
                    foreach (var pres in item.Pressure)
                    {
                        press.Pressure.Add(pres);
                    }

                    var windDirs = new WindDirections();
                    foreach (var windDir in item.WindDirection)
                    {
                        windDirs.WindDirection.Add(windDir);
                    }

                    var windSpeeds = new WindSpeeds();
                    foreach (var windSpeed in item.WindSpeed)
                    {
                        windSpeeds.WindSpeed.Add(windSpeed);
                    }

                    var hours = new Hours();
                    foreach (var hour in item.Hours)
                    {
                        hours.Hour.Add(hour);
                    }

                    newList.WeatherList.Add(new WeatherDataProto()
                    {
                        Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc).ToTimestamp(),
                        Temperatures = temps,
                        Humidities = hums,
                        Pressures = press,
                        WindDirections = windDirs,
                        WindSpeeds = windSpeeds,
                        Hours = hours
                    });

                }

                returnWeatherData.WeatherData.Add(new TargetDateWeather()
                {
                    TargetDate = DateTime.SpecifyKind(newWeatherData.TargetDate, DateTimeKind.Utc).ToTimestamp(),
                    Weather = newList
                });
            }

            return returnWeatherData;
        }
    }
}