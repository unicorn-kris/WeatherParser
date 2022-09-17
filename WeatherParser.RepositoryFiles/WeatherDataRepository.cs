using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository
{
    public class WeatherParserRepository : IWeatherParserRepository
    {
        public Dictionary<DateTime, List<WeatherRepository>> GetAllWeatherData(DateTime targetDate)
        {
            Dictionary<DateTime, List<WeatherRepository>> dataInFiles = new Dictionary<DateTime, List<WeatherRepository>>();
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
                            dataInFiles.Add(DateTime.Parse(tempStr[0]), new List<WeatherRepository>());
                        }


                        for (int i = 2; i < tempStr.Length; ++i)
                        {
                            WeatherRepository weatherData = new WeatherRepository()
                            {
                                Temperature = new List<double>(),
                                Humidity = new List<int>(),
                                Pressure = new List<int>(),
                                WindDirection = new List<string>(),
                                WindSpeed = new List<int>()
                            };
                            weatherData.Date = targetDate;

                            if (tempStr[i].Any(c => c == '−'))
                            {
                                weatherData.Temperature.Add(double.Parse(tempStr[i].Replace('−', ' ').Trim()) * -1);
                            }
                            else
                            {
                                weatherData.Temperature.Add(double.Parse(tempStr[i]));
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
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Pressure.Add(int.Parse(tempStr[i]));
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
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeed.Add(int.Parse(tempStr[i]));
                            }
                            else
                            {
                                dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindSpeed.Add(int.Parse(tempStr[i].Split('-')[0]));
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
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].WindDirection.Add(tempStr[i]);
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
                            dataInFiles[DateTime.Parse(tempStr[0])][indexWeatherData].Humidity.Add(int.Parse(tempStr[i]));
                            ++indexWeatherData;
                        }
                    }
                }
            }

            return dataInFiles;
        }

        public DateTime GetFirstDate()
        {
            return DateTime.Parse(File.ReadLines("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt").FirstOrDefault().Trim().Split(' ')[1]);
        }

        public DateTime GetLastDate()
        {
            return DateTime.Parse(File.ReadLines("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt").LastOrDefault().Trim().Split(' ')[1]);
        }

        public void SaveWeatherData(DateTime targetDate, WeatherRepository listOfWeatherData)
        {
            string pathMain = @"../WeatherParser.RepositoryFiles/SaveFiles";
            if (!Directory.Exists(pathMain))
            {
                Directory.CreateDirectory(pathMain);
            }

            if (!File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt")
                    .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == targetDate.Date))
            {
                using (StreamWriter fileTemperature = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Temperature.txt", true))
                {

                    fileTemperature.Write($"{targetDate.ToShortDateString()} ");
                    fileTemperature.Write($"{listOfWeatherData.Date.ToShortDateString()} ");
                    foreach (var tempOfWeatherData in listOfWeatherData.Temperature)
                    {
                        fileTemperature.Write($"{tempOfWeatherData} ");
                    }
                    fileTemperature.WriteLine();
                }
            }

            if (!File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/Pressure.txt")
                    .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == targetDate.Date))
            {
                using (StreamWriter filePressure = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Pressure.txt", true))
                {
                    filePressure.Write($"{targetDate.ToShortDateString()} ");
                    filePressure.Write($"{listOfWeatherData.Date.ToShortDateString()} ");
                    foreach (var presOfWeatherData in listOfWeatherData.Pressure)
                    {
                        filePressure.Write($"{presOfWeatherData} ");
                    }
                    filePressure.WriteLine();
                }
            }

            if (!File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/WindSpeed.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == targetDate.Date))
            {
                using (StreamWriter fileWindSpeed = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/WindSpeed.txt", true))
                {
                    fileWindSpeed.Write($"{targetDate.ToShortDateString()} ");
                    fileWindSpeed.Write($"{listOfWeatherData.Date.ToShortDateString()}");
                    foreach (var windspOfWeatherData in listOfWeatherData.WindSpeed)
                    {
                        fileWindSpeed.Write($" {windspOfWeatherData}");
                    }
                    fileWindSpeed.WriteLine();
                }
            }

            if (!File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/WindDirection.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == targetDate.Date))
            {
                using (StreamWriter fileWindDirection = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/WindDirection.txt", true))
                {
                    fileWindDirection.Write($"{targetDate.ToShortDateString()} ");
                    fileWindDirection.Write($"{listOfWeatherData.Date.ToShortDateString()} ");
                    foreach (var winddirOfWeatherData in listOfWeatherData.WindDirection)
                    {
                        fileWindDirection.Write($"{winddirOfWeatherData} ");
                    }
                    fileWindDirection.WriteLine();
                }
            }

            if (!File.ReadAllLines("../WeatherParser.RepositoryFiles/SaveFiles/Humidity.txt")
                .Any(c => DateTime.Parse(c.Trim().Split(' ')[1]) == targetDate.Date))
            {
                using (StreamWriter fileHumidity = new StreamWriter("../WeatherParser.RepositoryFiles/SaveFiles/Humidity.txt", true))
                {
                    fileHumidity.Write($"{targetDate.ToShortDateString()} ");
                    fileHumidity.Write($"{listOfWeatherData.Date.ToShortDateString()} ");
                    foreach (var humOfWeatherData in listOfWeatherData.Humidity)
                    {
                        fileHumidity.Write($"{humOfWeatherData} ");
                    }
                    fileHumidity.WriteLine();
                }
            }
        }
    }
}
