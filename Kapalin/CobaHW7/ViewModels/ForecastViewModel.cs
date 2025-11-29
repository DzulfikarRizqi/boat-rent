using CobaHW7.Class;
using CobaHW7.Services;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace CobaHW7.ViewModels
{
    public class ForecastViewModel : BaseViewModel
    {
        private ForecastData _forecastData;
        public ForecastData ForecastData
        {
            get => _forecastData;
            set
            {
                if (_forecastData != value)
                {
                    _forecastData = value;
                    OnPropertyChanged(nameof(ForecastData));
                    UpdateForecastDays();
                }
            }
        }

        private ObservableCollection<ForecastDay> _forecastDays;
        public ObservableCollection<ForecastDay> ForecastDays
        {
            get => _forecastDays;
            set
            {
                if (_forecastDays != value)
                {
                    _forecastDays = value;
                    OnPropertyChanged(nameof(ForecastDays));
                }
            }
        }

        private ForecastDay _selectedDay;
        public ForecastDay SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged(nameof(SelectedDay));
                    UpdateChartData();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public string LocationName { get; set; }

        public ICommand GoBackCommand { get; }
        public ICommand SelectTemperatureTabCommand { get; }
        public ICommand SelectPrecipitationTabCommand { get; }
        public ICommand SelectWindTabCommand { get; }

        private string _selectedChartType = "Temperature";
        public string SelectedChartType
        {
            get => _selectedChartType;
            set
            {
                if (_selectedChartType != value)
                {
                    _selectedChartType = value;
                    OnPropertyChanged(nameof(SelectedChartType));
                }
            }
        }

        private ObservableCollection<DayForecast> _sevenDayForecast;
        public ObservableCollection<DayForecast> SevenDayForecast
        {
            get => _sevenDayForecast;
            set
            {
                if (_sevenDayForecast != value)
                {
                    _sevenDayForecast = value;
                    OnPropertyChanged(nameof(SevenDayForecast));
                }
            }
        }

        private List<ChartPoint> _chartPoints;
        public List<ChartPoint> ChartPoints
        {
            get => _chartPoints;
            set
            {
                if (_chartPoints != value)
                {
                    _chartPoints = value;
                    OnPropertyChanged(nameof(ChartPoints));
                }
            }
        }

        private string _chartColor;
        public string ChartColor
        {
            get => _chartColor;
            set
            {
                if (_chartColor != value)
                {
                    _chartColor = value;
                    OnPropertyChanged(nameof(ChartColor));
                }
            }
        }

        private PlotModel _forecastPlotModel;
        public PlotModel ForecastPlotModel
        {
            get => _forecastPlotModel;
            set
            {
                if (_forecastPlotModel != value)
                {
                    _forecastPlotModel = value;
                    OnPropertyChanged(nameof(ForecastPlotModel));
                }
            }
        }

        public ForecastViewModel()
        {
            ForecastDays = new ObservableCollection<ForecastDay>();
            SevenDayForecast = new ObservableCollection<DayForecast>();
            ChartPoints = new List<ChartPoint>();
            ChartColor = "#FF9800"; // Default orange
            GoBackCommand = new RelayCommand(ExecuteGoBack);
            SelectTemperatureTabCommand = new RelayCommand(_ => SelectTab("Temperature"));
            SelectPrecipitationTabCommand = new RelayCommand(_ => SelectTab("Precipitation"));
            SelectWindTabCommand = new RelayCommand(_ => SelectTab("Wind"));
        }

        private void SelectTab(string tabType)
        {
            SelectedChartType = tabType;
            UpdateChartData();
        }

        public async void LoadForecast(string location)
        {
            LocationName = location;
            await LoadForecastAsync(location);
        }

        private async System.Threading.Tasks.Task LoadForecastAsync(string location)
        {
            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = "";

                var forecastData = await WeatherService.GetForecastAsync(location);

                if (forecastData == null)
                {
                    HasError = true;
                    ErrorMessage = $"Gagal memuat forecast untuk '{location}'";
                    ForecastData = null;
                }
                else
                {
                    ForecastData = forecastData;
                    if (ForecastDays.Count > 0)
                    {
                        SelectedDay = ForecastDays[0];
                    }
                    HasError = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ForecastViewModel] Error: {ex.Message}");
                HasError = true;
                ErrorMessage = $"Terjadi kesalahan: {ex.Message}";
                ForecastData = null;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateForecastDays()
        {
            ForecastDays.Clear();
            SevenDayForecast.Clear();

            if (ForecastData?.Forecast?.ForecastDays != null)
            {
                foreach (var day in ForecastData.Forecast.ForecastDays)
                {
                    ForecastDays.Add(day);
                }
                GenerateSevenDayForecast();
            }
        }

        private void GenerateSevenDayForecast()
        {
            if (ForecastDays.Count == 0) return;

            SevenDayForecast.Clear();
            var dayLabels = GetDynamicDayLabels();

            Debug.WriteLine($"[ForecastViewModel] Total forecast days from API: {ForecastDays.Count}");

            for (int i = 0; i < Math.Min(7, ForecastDays.Count); i++)
            {
                var forecastDay = ForecastDays[i];
                var dayForecast = new DayForecast
                {
                    DayLabel = dayLabels[i],
                    Date = forecastDay.Date,
                    MinTemp = (int)forecastDay.Day.MinTempC,
                    MaxTemp = (int)forecastDay.Day.MaxTempC,
                    WeatherCondition = forecastDay.Day.Condition.Text,
                    WeatherIcon = GetWeatherIcon(forecastDay.Day.Condition.Code)
                };
                SevenDayForecast.Add(dayForecast);
                Debug.WriteLine($"[ForecastViewModel] Day {i}: {dayLabels[i]} - {forecastDay.Date}");
            }
        }

        private List<string> GetDynamicDayLabels()
        {
            var labels = new List<string>();
            var dayNames = new[] { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };
            var today = DateTime.Now;
            var startDayOfWeek = (int)today.DayOfWeek;

            for (int i = 0; i < 7; i++)
            {
                var dayIndex = (startDayOfWeek + i) % 7;
                labels.Add(dayNames[dayIndex]); // Senin, Selasa, Rabu, dll (lengkap)
            }

            return labels;
        }

        private string GetWeatherIcon(int conditionCode)
        {
            // Weather codes dari weatherapi.com
            // 1000 = Sunny, 1003 = Partly cloudy, 1006 = Cloudy, 1009 = Overcast
            // 1183, 1186, 1189 = Light/Moderate/Heavy rain
            // 1192, 1195 = Heavy rain, 1198, 1201 = Light/Moderate freezing rain
            // 1243, 1246 = Heavy showers
            // 1273, 1276, 1279, 1282 = Thunderstorm

            if (conditionCode == 1000)
                return "☀️"; // Sunny
            else if (conditionCode >= 1003 && conditionCode <= 1009)
                return "☁️"; // Cloudy/Overcast
            else if ((conditionCode >= 1183 && conditionCode <= 1207) || (conditionCode >= 1243 && conditionCode <= 1246))
                return "🌧️"; // Rainy
            else if (conditionCode >= 1273 && conditionCode <= 1282)
                return "⛈️"; // Thunderstorm
            else if (conditionCode >= 1210 && conditionCode <= 1225)
                return "❄️"; // Snow
            else
                return "☁️"; // Default
        }

        private void ExecuteGoBack(object parameter)
        {

        }

        private void UpdateChartData()
        {
            if (SelectedDay?.Hours == null || SelectedDay.Hours.Count == 0)
            {
                ChartPoints = new List<ChartPoint>();
                ForecastPlotModel = null;
                return;
            }

            var points = new List<ChartPoint>();
            var filteredHours = SelectedDay.Hours.Where((h, i) => i % 3 == 0).ToList();
            var areaSeries = new AreaSeries();

            switch (SelectedChartType)
            {
                case "Temperature":
                    ChartColor = "#FF9800"; //oren
                    areaSeries.Fill = OxyColor.FromArgb(100, 255, 152, 0);
                    areaSeries.Color = OxyColor.FromArgb(255, 255, 152, 0);
                    foreach (var hour in filteredHours)
                    {
                        var timeLabel = hour.Time.Substring(0, 5);
                        var value = hour.TempC;
                        points.Add(new ChartPoint { TimeLabel = timeLabel, Value = value });
                        areaSeries.Points.Add(new DataPoint(filteredHours.IndexOf(hour), value));
                    }
                    break;

                case "Precipitation":
                    ChartColor = "#2196F3"; //bieu
                    areaSeries.Fill = OxyColor.FromArgb(100, 33, 150, 243);
                    areaSeries.Color = OxyColor.FromArgb(255, 33, 150, 243);
                    foreach (var hour in filteredHours)
                    {
                        var timeLabel = hour.Time.Substring(0, 5);
                        var value = hour.ChanceOfRain;
                        points.Add(new ChartPoint { TimeLabel = timeLabel, Value = value });
                        areaSeries.Points.Add(new DataPoint(filteredHours.IndexOf(hour), value));
                    }
                    break;

                case "Wind":
                    ChartColor = "#4CAF50"; //ijo
                    areaSeries.Fill = OxyColor.FromArgb(100, 76, 175, 80);
                    areaSeries.Color = OxyColor.FromArgb(255, 76, 175, 80);
                    foreach (var hour in filteredHours)
                    {
                        var timeLabel = hour.Time.Substring(0, 5);
                        var value = hour.WindKph;
                        points.Add(new ChartPoint { TimeLabel = timeLabel, Value = value });
                        areaSeries.Points.Add(new DataPoint(filteredHours.IndexOf(hour), value));
                    }
                    break;
            }

            ChartPoints = points;
            var plotModel = new PlotModel { Background = OxyColor.FromArgb(255, 255, 255, 255) };
            plotModel.Series.Add(areaSeries);

            string yAxisTitle = SelectedChartType switch
            {
                "Temperature" => "Suhu (°C)",
                "Precipitation" => "Kemungkinan Hujan (%)",
                "Wind" => "Kecepatan Angin (kph)",
                _ => ""
            };

            double majorStep = SelectedChartType switch
            {
                "Temperature" => 5,      
                "Precipitation" => 10,  
                "Wind" => 5,             
                _ => 10
            };

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = yAxisTitle,
                TitleFontSize = 12,
                MajorStep = majorStep,
                MajorGridlineStyle = OxyPlot.LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromArgb(200, 230, 230, 230),
                MinorTickSize = 0,  
                StringFormat = "0"   
            });

            var xAxis = new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Waktu (Jam)",
                TitleFontSize = 12,
                Minimum = -0.5,
                Maximum = filteredHours.Count - 0.5,
                MajorStep = 1,
                MinorStep = 1,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            plotModel.Axes.Add(xAxis);

            for (int i = 0; i < filteredHours.Count; i++)
            {
                var timeLabel = filteredHours[i].Time.Substring(0, 5);
                var annotation = new OxyPlot.Annotations.TextAnnotation
                {
                    Text = timeLabel,
                    TextPosition = new DataPoint(i, 0),
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Top,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                    FontSize = 11,
                    TextColor = OxyColor.FromArgb(255, 100, 100, 100)
                };
                plotModel.Annotations.Add(annotation);
            }

            ForecastPlotModel = plotModel;
            Debug.WriteLine($"[ForecastViewModel] Chart updated: {SelectedChartType} with {points.Count} points");
        }
    }
}
