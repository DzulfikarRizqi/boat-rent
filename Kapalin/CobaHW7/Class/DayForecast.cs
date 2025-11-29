namespace CobaHW7.Class
{
    public class DayForecast
    {
        public string DayLabel { get; set; } //senin,selasa,rabu,sampe minggu
        public string Date { get; set; }
        public int MinTemp { get; set; }
        public int MaxTemp { get; set; }
        public string WeatherCondition { get; set; }
        public string WeatherIcon { get; set; } // ☀️, ☁️, 🌧️, ⛈️
    }
}
