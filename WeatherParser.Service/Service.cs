using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Entities;
using Excel = Microsoft.Office.Interop.Excel;

namespace WeatherParser.Service
{
    public class Service : IService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public Service(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public async Task<List<WeatherDataService>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataByDayAsync(targetDate, siteId).ConfigureAwait(false);

            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();

            foreach (var weatherData in data)
            {
                var weathers = new List<WeatherService>();

                foreach (var weather in weatherData.Weather)
                {
                    weathers.Add(new WeatherService()
                    {
                        Hours = weather.Hours,
                        Date = weather.Date,
                        Humidity = weather.Humidity,
                        Pressure = weather.Pressure,
                        Temperature = weather.Temperature,
                        WindSpeed = weather.WindSpeed
                    });
                }
                weatherDataList.Add(new WeatherDataService()
                {
                    SiteId = weatherData.SiteID,
                    TargetDate = weatherData.TargetDate,
                    Weather = weathers
                });
            }

            weatherDataList.Sort((x, y) => x.TargetDate.CompareTo(y.TargetDate));

            return weatherDataList;
        }

        public async Task<(DateTime firstDate, DateTime lastDate)> GetFirstAndLastDateAsync(Guid siteId)
        {
            return await _weatherParserRepository.GetFirstAndLastDateAsync(siteId).ConfigureAwait(false);
        }

        public async Task<List<SiteService>> GetSitesAsync()
        {
            var repositorySites = await _weatherParserRepository.GetSitesAsync().ConfigureAwait(false);

            //map siterepository to siteservice
            var sites = new List<SiteService>();

            foreach (var site in repositorySites)
            {
                sites.Add(new SiteService()
                {
                    ID = site.ID,
                    Name = site.Name,
                    Rating = site.Rating
                });
            }
            return sites;
        }

        //deviations of real from forecast weather data
        public async Task<List<WeatherDataService>> GetDeviationsOfRealFromForecast(DateTime targetDate, Guid siteId)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataByDayAsync(targetDate, siteId).ConfigureAwait(false);

            if (data.Any(x => x.TargetDate.Date.Equals(targetDate.Date)))
            {
                return GetDeviations(data, targetDate);
            }
            else
            {
                throw new Exception($"Have no real weather on {targetDate.Date.ToShortDateString()}");
            }
        }

        //средние отклонения по числу дней прогноза для сайта
        public async Task<List<WeatherDataService>> GetMeanDeviationsOfRealForecast(Guid siteId, int days)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataBySiteAsync(siteId).ConfigureAwait(false);
            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();
            var countOfDates = 0;


            var dates = new List<DateTime>();

            foreach (var dateInGraph in data.Select(x => x.TargetDate.Date))
            {
                //собирались ли фактические данные на этот день
                if (data.Any(x => x.TargetDate.Date.Equals(dateInGraph.Date)))
                {
                    if (data.Where(x => x.Weather.Where(y => y.Date.Date.Equals(dateInGraph)).Count() == 1).Count() >= days)
                    {
                        dates.Add(dateInGraph);

                    }
                }
            }

            foreach (var date in dates)
            {
                countOfDates = 0;

                var weatherByDay = await _weatherParserRepository.GetAllWeatherDataByDayAsync(date, siteId).ConfigureAwait(false);

                //для каждой подошедшей даты вычисляю отклонения прогнозов от фактов
                var weatherDataListDeviations = GetDeviations(weatherByDay, date);

                if (weatherDataListDeviations.Count > days)
                {
                    weatherDataListDeviations.RemoveRange(0, weatherDataListDeviations.Count - days);
                }

                foreach (var weatherData in weatherDataListDeviations)
                {
                    if (countOfDates < days)
                    {
                        var weathers = new List<WeatherService>();

                        //если данные добавляются первый раз, то просто заношу их в список
                        if (!weathers.Any())
                        {
                            foreach (var weatherDat in weatherDataListDeviations)
                            {
                                foreach (var weather in weatherDat.Weather)
                                {
                                    weathers.Add(weather);
                                }
                            }
                        }
                        else
                        {
                            var humidities = new List<double>();
                            var temperatures = new List<double>();
                            var pressures = new List<double>();
                            var windSpeeds = new List<double>();

                            //all arrays of weather have a one size for one site
                            for (int j = 0; j < weatherData.Weather[0].Temperature.Count; j++)
                            {
                                temperatures.Add(weathers[countOfDates].Temperature[j] + weatherData.Weather[0].Temperature[j]);
                                humidities.Add(weathers[countOfDates].Humidity[j] + weatherData.Weather[0].Humidity[j]);
                                pressures.Add(weathers[countOfDates].Pressure[j] + weatherData.Weather[0].Pressure[j]);
                                windSpeeds.Add(weathers[countOfDates].WindSpeed[j] + weatherData.Weather[0].WindSpeed[j]);
                            }
                        }
                        for (int j = 0; j < weatherData.Weather[0].Temperature.Count; ++j)
                        {
                            weathers[countOfDates].Temperature[j] += weatherData.Weather[0].Temperature[j];
                            weathers[countOfDates].Pressure[j] += weatherData.Weather[0].Pressure[j];
                            weathers[countOfDates].Humidity[j] += weatherData.Weather[0].Humidity[j];
                            weathers[countOfDates].WindSpeed[j] += weatherData.Weather[0].WindSpeed[j];
                        }



                        if (weatherDataList.Count < days)
                        {
                            weatherDataList.Add(new WeatherDataService()
                            {
                                SiteId = siteId,
                                TargetDate = new DateTime(1, 1, countOfDates + 1),
                                Weather = new List<WeatherService>() { weathers[countOfDates] }
                            });
                        }
                        else
                        {
                            weatherDataList[countOfDates].Weather = new List<WeatherService>() { weathers[countOfDates] };
                        }

                        ++countOfDates;
                    }
                }
            }


            for (int i = 0; i < weatherDataList.Count; ++i)
            {
                var weather = weatherDataList[i].Weather[0];

                weather.Temperature = weather.Temperature.Select(x => 1.0 * x / dates.Count).ToList();
                weather.Humidity = weather.Humidity.Select(x => 1.0 * x / dates.Count).ToList();
                weather.Pressure = weather.Pressure.Select(x => 1.0 * x / dates.Count).ToList();
                weather.WindSpeed = weather.WindSpeed.Select(x => 1.0 * x / dates.Count).ToList();

            }
            return weatherDataList;
        }

        public Task<bool> HaveRealDataOnDay(DateTime targetDate, Guid siteId)
        {
            return _weatherParserRepository.HaveRealDataOnDay(targetDate, siteId);
        }

        public async Task SaveDataInExcel(string path, Guid siteId)
        {
            var dataFromdb = await _weatherParserRepository.GetAllWeatherDataBySiteAsync(siteId).ConfigureAwait(false);

            var dataForSaving = new List<ExcelWeatherDataFull>();

            foreach (var data in dataFromdb)
            {
                foreach (var weather in data.Weather)
                {
                    if (dataForSaving.Any(x => x.CurrentDate.Date == weather.Date.Date))
                    {
                        dataForSaving.Where(x => x.CurrentDate.Date == weather.Date.Date).First().Weather.Add(new ExcelWeather()
                        {
                            Date = data.TargetDate,
                            Hours = weather.Hours,
                            Humidity = weather.Humidity,
                            Pressure = weather.Pressure,
                            Temperature = weather.Temperature,
                            WindSpeed = weather.WindSpeed

                        });
                    }
                    else
                    {
                        dataForSaving.Add(new ExcelWeatherDataFull()
                        {
                            CurrentDate = weather.Date,
                            Weather = new List<ExcelWeather>() {
                                new ExcelWeather() {
                                    Date = data.TargetDate,
                                    Hours = weather.Hours,
                                    Humidity = weather.Humidity,
                                    Pressure = weather.Pressure,
                                    Temperature = weather.Temperature,
                                    WindSpeed = weather.WindSpeed
                                }
                            }
                        });
                    }
                }
            }

            Excel.Application excelApp = null;

            int worksheetCount = 0;

            //create new instance
            excelApp = new Excel.Application();

            excelApp.Visible = false;

            //create new workbook
            var workbook = excelApp.Workbooks.Add();

            //get number of existing worksheets
            worksheetCount = workbook.Sheets.Count;

            //add a worksheet and set the value to the new worksheet
            var worksheetTemperature = (Excel.Worksheet)workbook.Sheets.Add();
            var worksheetPressure = (Excel.Worksheet)workbook.Sheets.Add();
            var worksheetHumidity = (Excel.Worksheet)workbook.Sheets.Add();
            var worksheetWindSpeed = (Excel.Worksheet)workbook.Sheets.Add();

            var rowNum = 0;
            if (dataForSaving.Any())
            {
                foreach (var data in dataForSaving)
                {
                    for (int j = 0; j < data.Weather.Count; j++)
                    {
                        if (j == 0)
                        {
                            worksheetTemperature.Cells[rowNum, 0] = data.CurrentDate;
                            worksheetPressure.Cells[rowNum, 0] = data.CurrentDate;
                            worksheetHumidity.Cells[rowNum, 0] = data.CurrentDate;
                            worksheetWindSpeed.Cells[rowNum, 0] = data.CurrentDate;
                        }

                        for (int k = -1; k < data.Weather[j].Hours.Count; k++)
                        {
                            //set value of cell
                            if (k == -1)
                            {
                                worksheetTemperature.Cells[rowNum, 1] = data.Weather[j].Date;
                                worksheetPressure.Cells[rowNum, 1] = data.Weather[j].Date;
                                worksheetHumidity.Cells[rowNum, 1] = data.Weather[j].Date;
                                worksheetWindSpeed.Cells[rowNum, 1] = data.Weather[j].Date;
                            }
                            else
                            {
                                worksheetTemperature.Cells[rowNum, k] = data.Weather[j].Temperature[k];
                                worksheetPressure.Cells[rowNum, k] = data.Weather[j].Pressure[k];
                                worksheetHumidity.Cells[rowNum, k] = data.Weather[j].Humidity[k];
                                worksheetWindSpeed.Cells[rowNum, k] = data.Weather[j].WindSpeed[k];
                            }
                        }
                    }
                    ++rowNum;
                }
            }

            if (workbook != null)
            {
                //save Workbook - if file exists, overwrite it
                workbook.SaveAs(path);

                //close workbook
                workbook.Close();

                //release all resources
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
            }

            if (excelApp != null)
            {
                //close Excel
                excelApp.Quit();

                //release all resources
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);
            }
        }

        private List<WeatherDataService> GetDeviations(List<WeatherDataRepository> data, DateTime targetDate)
        {
            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();

            var targetDataDate = data
                .Where(x => x.TargetDate.Date.Equals(targetDate.Date))
                .FirstOrDefault();

            if (targetDataDate != null) {
                var targetData = targetDataDate
                        .Weather
                        .Where(x => x.Date.Date.Equals(targetDate.Date))
                        .FirstOrDefault();

                foreach (var weatherData in data)
                {
                    var weathers = new List<WeatherService>();

                    foreach (var weather in weatherData.Weather)
                    {
                        var humidities = new List<double>();
                        var temperatures = new List<double>();
                        var pressures = new List<double>();
                        var windSpeeds = new List<double>();
                        var hours = new List<int>();

                        //all arrays of weather have a one size for one site
                        for (int i = 0; i < (weather.Temperature.Count < targetData.Temperature.Count ? weather.Temperature.Count : targetData.Temperature.Count); i++)
                        {
                            if (targetData.Hours.Contains(weather.Hours[i]))
                            {
                                temperatures.Add(targetData.Temperature[i] - weather.Temperature[i]);
                                humidities.Add(targetData.Humidity[i] - weather.Humidity[i]);
                                pressures.Add(targetData.Pressure[i] - weather.Pressure[i]);
                                windSpeeds.Add(targetData.WindSpeed[i] - weather.WindSpeed[i]);
                                hours.Add(weather.Hours[i]);
                            }
                        }

                        weathers.Add(new WeatherService()
                        {
                            Hours = hours,
                            Date = weather.Date,
                            Humidity = humidities,
                            Pressure = pressures,
                            Temperature = temperatures,
                            WindSpeed = windSpeeds
                        });
                    }
                    weatherDataList.Add(new WeatherDataService()
                    {
                        SiteId = weatherData.SiteID,
                        TargetDate = weatherData.TargetDate,
                        Weather = weathers
                    });
                }
            }

            return weatherDataList;
        }

        private class ExcelWeatherDataFull
        {
            public DateTime CurrentDate { get; set; }

            public List<ExcelWeather> Weather { get; set; }
        }

        private class ExcelWeather
        {
            public DateTime Date { get; set; }

            public List<int> Hours { get; set; }

            public List<double> Temperature { get; set; }

            public List<double> Pressure { get; set; }

            public List<double> Humidity { get; set; }

            public List<double> WindSpeed { get; set; }
        }
    }
}