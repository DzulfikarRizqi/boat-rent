using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CobaHW7.Services
{
    public class WeatherData
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }
    }

    public class Location
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }

    public class Current
    {
        [JsonProperty("temp_c")]
        public double TemperatureCelsius { get; set; }

        [JsonProperty("temp_f")]
        public double TemperatureFahrenheit { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("wind_kph")]
        public double WindKph { get; set; }

        [JsonProperty("wind_mph")]
        public double WindMph { get; set; }

        [JsonProperty("pressure_mb")]
        public double PressureMb { get; set; }

        [JsonProperty("cloud")]
        public int Cloud { get; set; }
    }

    public class Condition
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class ForecastData
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("current")]
        public Current Current { get; set; }

        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }

    public class Forecast
    {
        [JsonProperty("forecastday")]
        public List<ForecastDay> ForecastDays { get; set; }
    }

    public class ForecastDay
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        public DayData Day { get; set; }

        [JsonProperty("hour")]
        public List<Hour> Hours { get; set; }
    }

    public class DayData
    {
        [JsonProperty("maxtemp_c")]
        public double MaxTempC { get; set; }

        [JsonProperty("mintemp_c")]
        public double MinTempC { get; set; }

        [JsonProperty("avgtemp_c")]
        public double AvgTempC { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("avghumidity")]
        public double AvgHumidity { get; set; }

        [JsonProperty("avgvis_km")]
        public double AvgVisKm { get; set; }

        [JsonProperty("will_it_rain")]
        public int WillItRain { get; set; }

        [JsonProperty("chance_of_rain")]
        public int ChanceOfRain { get; set; }
    }

    public class Hour
    {
        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("temp_c")]
        public double TempC { get; set; }

        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("wind_kph")]
        public double WindKph { get; set; }

        [JsonProperty("will_it_rain")]
        public int WillItRain { get; set; }

        [JsonProperty("chance_of_rain")]
        public int ChanceOfRain { get; set; }
    }

    public class WeatherService
    {
        private static readonly string apiKey = "d2ed3ac229db428bab5173412251509";
        private static readonly string baseUrl = "https://api.weatherapi.com/v1/current.json";
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Fetch current weather data untuk lokasi tertentu
        /// </summary>
        /// <param name="location">Nama kota atau pantai (contoh: "Bali", "Jakarta", "Lombok")</param>
        /// <returns>WeatherData object atau null jika gagal</returns>
        public static async Task<WeatherData> GetWeatherAsync(string location)
        {
            try
            {
                if (string.IsNullOrEmpty(location))
                {
                    Debug.WriteLine("[WeatherService] Location kosong");
                    return null;
                }

                string url = $"{baseUrl}?key={apiKey}&q={Uri.EscapeDataString(location)}&aqi=no";

                Debug.WriteLine($"[WeatherService] Fetching weather untuk: {location}");

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[WeatherService] Error: {response.StatusCode}");
                    return null;
                }

                string content = await response.Content.ReadAsStringAsync();
                WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(content);

                Debug.WriteLine($"[WeatherService] Success - Location: {weatherData?.Location?.Name}, Temp: {weatherData?.Current?.TemperatureCelsius}°C");

                return weatherData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WeatherService] Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Fetch forecast weather data untuk 7 hari ke depan dengan hourly forecast
        /// </summary>
        /// <param name="location">Nama kota atau pantai (contoh: "Bali", "Jakarta", "Lombok")</param>
        /// <returns>ForecastData object atau null jika gagal</returns>
        public static async Task<ForecastData> GetForecastAsync(string location)
        {
            try
            {
                if (string.IsNullOrEmpty(location))
                {
                    Debug.WriteLine("[WeatherService] Location kosong");
                    return null;
                }

                string forecastUrl = "https://api.weatherapi.com/v1/forecast.json";
                string url = $"{forecastUrl}?key={apiKey}&q={Uri.EscapeDataString(location)}&days=7&aqi=no&alerts=no";

                Debug.WriteLine($"[WeatherService] Fetching forecast untuk: {location}");

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"[WeatherService] Error: {response.StatusCode}");
                    return null;
                }

                string content = await response.Content.ReadAsStringAsync();
                ForecastData forecastData = JsonConvert.DeserializeObject<ForecastData>(content);

                Debug.WriteLine($"[WeatherService] Success - Forecast loaded for: {forecastData?.Location?.Name}");

                return forecastData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[WeatherService] Exception: {ex.Message}");
                return null;
            }
        }
    }
}
