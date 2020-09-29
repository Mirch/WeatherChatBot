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
                Temperature = 20
            },
            new WeatherInfo
            {
                Description = "cloudy",
                Temperature = 10
            },
            new WeatherInfo
            {
                Description = "raining",
                Temperature = 15
            },  
            new WeatherInfo
            {
                Description = "snowing",
                Temperature = -6
            },

        };

        private readonly Random _random = new Random();

        public WeatherInfo GetMockWeatherInfo()
        {
            var weather = _weatherInfo[_random.Next(_weatherInfo.Count)];
            weather.Temperature += _random.Next(11) - 5;

            return weather;
        }
    }
}
