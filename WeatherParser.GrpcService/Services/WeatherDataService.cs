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

        public override Task<WeatherDataGetResponse> GetAllWeatherData(WeatherDataRequest request, ServerCallContext context)
        {
            try
            {
                List<Service.Entities.WeatherDataService> weatherData = _weatherParserService.GetAllWeatherData(request.Date.ToDateTime(), new Guid(request.SiteID));

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

                        newList.WeatherList.Add(new WeatherDataProto()
                        {
                            Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc).ToTimestamp(),
                            Temperatures = temps,
                            Humidities = hums,
                            Pressures = press,
                            WindDirections = windDirs,
                            WindSpeeds = windSpeeds
                        });

                    }

                    returnWeatherData.WeatherData.Add(new TargetDateWeather()
                    {
                        TargetDate = DateTime.SpecifyKind(newWeatherData.TargetDate, DateTimeKind.Utc).ToTimestamp(),
                        Weather = newList
                    });
                }

                return Task.FromResult(returnWeatherData);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async override Task<FirstLastDates> GetFirstAndLastDate(SiteID request, ServerCallContext context)
        {
            try
            {
                var dates = _weatherParserService.GetFirstAndLastDate(new Guid(request.SiteID_));

                var result = new FirstLastDates();

                foreach (var date in dates)
                {
                    result.Dates.Add(DateTime.SpecifyKind(date, DateTimeKind.Utc).ToTimestamp());
                }
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirstAndLastDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async override Task<Empty> SaveWeatherData(Empty request, ServerCallContext context)
        {
            try
            {
                _weatherParserService.SaveWeatherData();
                return await Task.FromResult(new Empty());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}