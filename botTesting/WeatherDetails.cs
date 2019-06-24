using System;
using System.Collections.Generic;

namespace botTesting
{
    [Serializable]
    public class WeatherDetails
    {
        [Serializable]
        public class ConsolidatedWeather
        {
            public object id { get; set; }
            public string weather_state_name { get; set; }
            public string weather_state_abbr { get; set; }
            public string wind_direction_compass { get; set; }
            public DateTime created { get; set; }
            public string applicable_date { get; set; }
            public double min_temp { get; set; }
            public double max_temp { get; set; }
            public double the_temp { get; set; }
            public double wind_speed { get; set; }
            public double wind_direction { get; set; }
            public double air_pressure { get; set; }
            public int humidity { get; set; }
            public double visibility { get; set; }
            public int predictability { get; set; }
        }

        [Serializable]
        public class Parent
        {
            public string title { get; set; }
            public string location_type { get; set; }
            public int woeid { get; set; }
            public string latt_long { get; set; }
        }

        [Serializable]
        public class Source
        {
            public string title { get; set; }
            public string slug { get; set; }
            public string url { get; set; }
            public int crawl_rate { get; set; }
        }

        [Serializable]
        public class RootObject
        {
            public List<ConsolidatedWeather> consolidated_weather { get; set; }
            public DateTime time { get; set; }
            public DateTime sun_rise { get; set; }
            public DateTime sun_set { get; set; }
            public string timezone_name { get; set; }
            public Parent parent { get; set; }
            public List<Source> sources { get; set; }
            public string title { get; set; }
            public string location_type { get; set; }
            public int woeid { get; set; }
            public string latt_long { get; set; }
            public string timezone { get; set; }
        }
    }
}