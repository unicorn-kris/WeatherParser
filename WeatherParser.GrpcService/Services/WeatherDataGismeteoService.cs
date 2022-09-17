using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherParser.Service.Entities;
using WeatherParser.Servicee.Contract.Graphics;

namespace WeatherParser.GrpcService.Services
{
    public class WeatherDataGismeteoService : WeatherDataProtoGismeteo.WeatherDataProtoGismeteoBase
    {
        private readonly ILogger<WeatherDataGismeteoService> _logger;
        private readonly IWeatherParserServiceGismeteo _weatherParserService;

        public WeatherDataGismeteoService(ILogger<WeatherDataGismeteoService> logger, IWeatherParserServiceGismeteo weatherParserServiceGismeteo)
        {
            _logger = logger;
            _weatherParserService = weatherParserServiceGismeteo;
        }

        public override Task<WeatherDataGetResponse> GetAllWeatherData(Timestamp request, ServerCallContext context)
        {
            try
            {
                Dictionary<DateTime, List<WeatherService>> weatherData = _weatherParserService.GetAllWeatherData(request.ToDateTime());

                var returnWeatherDataDict = new WeatherDataGetResponse();

                foreach (var newWeatherData in weatherData)
                {
                    var newList = new weatherDataList();

                    foreach (var item in newWeatherData.Value)
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

                        newList.WeatherDataList.Add(new WeatherDataProto()
                        {
                            Date = DateTime.SpecifyKind(item.Date, DateTimeKind.Utc).ToTimestamp(),
                            Temperatures = temps,
                            Humidities = hums,
                            Pressures = press,
                            WindDirections = windDirs,
                            WindSpeeds = windSpeeds
                        });

                    }

                    returnWeatherDataDict.WeatherDataDictionary.Add(new KeyValuePair()
                    {
                        Key = DateTime.SpecifyKind(newWeatherData.Key, DateTimeKind.Utc).ToTimestamp(),
                        Value = newList
                    });
                }

                return Task.FromResult(returnWeatherDataDict);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async override Task<Timestamp> GetFirstDate(Empty request, ServerCallContext context)
        {
            try
            {
                return await Task.FromResult(DateTime.SpecifyKind(_weatherParserService.GetFirstDate(), DateTimeKind.Utc).ToTimestamp());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirstDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async override Task<Timestamp> GetLastDate(Empty request, ServerCallContext context)
        {
            try
            {
                return await Task.FromResult(DateTime.SpecifyKind(_weatherParserService.GetLastDate(), DateTimeKind.Utc).ToTimestamp());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLastDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public async override Task<Empty> SaveWeatherData(WeatherDataSaveRequest request, ServerCallContext context)
        {
            try
            {
                _weatherParserService.SaveWeatherData(request.Url, request.Day);
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