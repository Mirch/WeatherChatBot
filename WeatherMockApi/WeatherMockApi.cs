using System;
using System.Collections.Generic;

namespace CoreBot.WeatherMockApi
{
    public class WeatherMockApi
    {
        private readonly List<WeatherInfo> _weatherInfo = new List<WeatherInfo>
        {
            new WeatherInfo
            {
                Description = "sunny",
                Temperature = 20,
                WindSpeed = 5
            },
            new WeatherInfo
            {
                Description = "cloudy",
                Temperature = 10,
                WindSpeed = 9
            },
            new WeatherInfo
            {
                Description = "raining",
                Temperature = 15,
                WindSpeed = 13
            },  
            new WeatherInfo
            {
                Description = "snowing",
                Temperature = -6,
                WindSpeed = 4
            },

        };

        private static readonly Dictionary<string, WeatherInfo> _cityInfos;

        private readonly Random _random = new Random();

        public WeatherInfo GetMockWeatherInfo(string city)
        {
            if(_cityInfos.ContainsKey(city))
            {
                return _cityInfos[city];
            }

            var weather = _weatherInfo[_random.Next(_weatherInfo.Count)];
            weather.Temperature += _random.Next(11) - 5;

            _cityInfos.Add(city, weather);

            return weather;
        }
    }
}
