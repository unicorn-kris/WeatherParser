using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherParser.Repository.Entities;
using WeatherParser.Repository.Contract;

namespace WeatherParser.Repository
{
    public class WeatherParserRepository : IWeatherParserRepository
    {
        public Dictionary<DateTime, List<WeatherDataRepository>> GetAllWeatherData(DateTime targetDate)
        {
            Dictionary<DateTime, List<WeatherDataRepository>> dataInFiles = new Dictionary<DateTime, List<WeatherDataRepository>>();
            //когда был составлен прогноз + список, где каждый список это weatherData на каждый из 8 часов

            using (StreamReader fileTemperature = new StreamReader("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt"))
            {
                while (!fileTemperature.EndOfStream)
                {
                    var tempStr = fileTemperature.ReadLine().Trim().Split(' ');

                    if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                    {
                        if (!dataInFiles.ContainsKey(DateTime.Parse(tempStr[0])))
                        {
                            dataInFiles.Add(DateTime.Parse(tempStr[0]), new List<WeatherDataRepository>());
                        }


                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            WeatherDataRepository weatherData = new WeatherDataRepository();
                            weatherData.CollectionDate = DateTime.Parse(tempStr[0]);
                            weatherData.Date = targetDate;

                            if (tempStr[i].Any(c => c == '−'))
                            {
                                weatherData.Temperature = double.Parse(tempStr[i].Replace('−', ' ').Trim()) * -1;
                            }
                            else
                            {
                                weatherData.Temperature = double.Parse(tempStr[i]);
                            }

                            dataInFiles[DateTime.Parse(tempStr[0])].Add(weatherData);
                        }
                    }
                }
            }

            using (StreamReader filePressure = new StreamReader("../WeatherParser.RepositoryFiles/SaveFiles/Pressure.txt"))
            {
                while (!filePressure.EndOfStream)
                {
                    var tempStr = filePressure.ReadLine().Trim().Split(' ');

                    if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                    {
                        int indexWeatherData = 0;
                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Pressure = int.Parse(tempStr[i]);
                            ++indexWeatherData;
                        }
                    }
                }
            }

            using (StreamReader fileWindSpeed = new StreamReader("../WeatherParser.RepositoryFiles/SaveFiles/WindSpeed.txt"))
            {
                while (!fileWindSpeed.EndOfStream)
                {
                    var tempStr = fileWindSpeed.ReadLine().Trim().Split(' ');

                    if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                    {
                        int indexWeatherData = 0;
                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            if (!tempStr[i].Any(c => c == '-'))
                            {
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeedFirst = int.Parse(tempStr[i]);
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeedSecond = int.MaxValue;
                            }
                            else
                            {
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeedFirst = int.Parse(tempStr[i].Split('-')[0]);
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeedSecond = int.Parse(tempStr[i].Split('-')[1]);
                            }
                            ++indexWeatherData;
                        }
                    }
                }
            }

            using (StreamReader fileWindDirection = new StreamReader("../WeatherParser.RepositoryFiles/SaveFiles/WindDirection.txt"))
            {
                while (!fileWindDirection.EndOfStream)
                {
                    var tempStr = fileWindDirection.ReadLine().Trim().Split(' ');

                    if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                    {
                        int indexWeatherData = 0;
                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindDirection = tempStr[i];
                            ++indexWeatherData;
                        }
                    }
                }
            }

            using (StreamReader fileHumidity = new StreamReader("../WeatherParser.RepositoryFiles/SaveFiles/Humidity.txt"))
            {
                while (!fileHumidity.EndOfStream)
                {
                    var tempStr = fileHumidity.ReadLine().Trim().Split(' ');

                    if (DateTime.Parse(tempStr[1]) == targetDate.Date)
                    {
                        int indexWeatherData = 0;
                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Humidity = int.Parse(tempStr[i]);
                            ++indexWeatherData;
                        }
                    }
                }
            }

            return dataInFiles;
        }

        public DateTime GetFirstDate()
        {
            return DateTime.Parse(File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt").FirstOrDefault().Trim().Split(' ')[1]).ToUniversalTime();
        }

        public DateTime GetLastDate()
        {
            return DateTime.Parse(File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt").LastOrDefault().Trim().Split(' ')[1]).ToUniversalTime();
        }

        public void SaveWeatherData(List<WeatherDataRepository> listOfWeatherData)
        {
            string pathMain = @"../WeatherParser.RepositoryFiles/SaveFiles";
            if (!Directory.Exists(pathMain))
            {
                Directory.CreateDirectory(pathMain);
            }

            try
            {
                using (StreamWriter fileTemperature = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt", true))
                {
                    fileTemperature.Write($"{listOfWeatherData[0].CollectionDate.ToShortDateString()} ");
                    fileTemperature.Write($"{listOfWeatherData[0].Date.ToShortDateString()} ");
                    foreach (var tempOfWeatherData in listOfWeatherData)
                    {
                        fileTemperature.Write($"{tempOfWeatherData.Temperature} ");
                    }
                    fileTemperature.WriteLine();
                }

                using (StreamWriter filePressure = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Pressure.txt", true))
                {

                    filePressure.Write($"{listOfWeatherData[0].CollectionDate.ToShortDateString()} ");
                    filePressure.Write($"{listOfWeatherData[0].Date.ToShortDateString()} ");
                    foreach (var tempOfWeatherData in listOfWeatherData)
                    {
                        filePressure.Write($"{tempOfWeatherData.Pressure} ");
                    }
                    filePressure.WriteLine();
                }

                using (StreamWriter fileWindSpeed = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/WindSpeed.txt", true))
                {
                    fileWindSpeed.Write($"{listOfWeatherData[0].CollectionDate.ToShortDateString()} ");
                    fileWindSpeed.Write($"{listOfWeatherData[0].Date.ToShortDateString()}");
                    foreach (var tempOfWeatherData in listOfWeatherData)
                    {
                        fileWindSpeed.Write($" {tempOfWeatherData.WindSpeedFirst}");
                        if (tempOfWeatherData.WindSpeedSecond != int.MaxValue)
                        {
                            fileWindSpeed.Write($"-{tempOfWeatherData.WindSpeedSecond}");
                        }
                    }
                    fileWindSpeed.WriteLine();
                }

                using (StreamWriter fileWindDirection = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/WindDirection.txt", true))
                {
                    fileWindDirection.Write($"{listOfWeatherData[0].CollectionDate.ToShortDateString()} ");

                    fileWindDirection.Write($"{listOfWeatherData[0].Date.ToShortDateString()} ");
                    foreach (var tempOfWeatherData in listOfWeatherData)
                    {
                        fileWindDirection.Write($"{tempOfWeatherData.WindDirection} ");
                    }
                    fileWindDirection.WriteLine();
                }

                using (StreamWriter fileHumidity = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Humidity.txt", true))
                {
                    fileHumidity.Write($"{listOfWeatherData[0].CollectionDate.ToShortDateString()} ");

                    fileHumidity.Write($"{listOfWeatherData[0].Date.ToShortDateString()} ");
                    foreach (var tempOfWeatherData in listOfWeatherData)
                    {
                        fileHumidity.Write($"{tempOfWeatherData.Humidity} ");
                    }
                    fileHumidity.WriteLine();
                }
            }
            catch (Exception e)
            {
                using (StreamWriter fileError = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Errors.txt", true))
                {
                    fileError.WriteLine(e.Message);
                }
            }
        }
    }
}
