using CobaHW7.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
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
                    UpdateChart();
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

        private PlotModel _chartModel;
        public PlotModel ChartModel
        {
            get => _chartModel;
            set
            {
                if (_chartModel != value)
                {
                    _chartModel = value;
                    OnPropertyChanged(nameof(ChartModel));
                }
            }
        }

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
                    UpdateChart();
                }
            }
        }

        public ForecastViewModel()
        {
            ForecastDays = new ObservableCollection<ForecastDay>();
            GoBackCommand = new RelayCommand(ExecuteGoBack);
            SelectTemperatureTabCommand = new RelayCommand(_ => SelectedChartType = "Temperature");
            SelectPrecipitationTabCommand = new RelayCommand(_ => SelectedChartType = "Precipitation");
            SelectWindTabCommand = new RelayCommand(_ => SelectedChartType = "Wind");
            ChartModel = new PlotModel();
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
            if (ForecastData?.Forecast?.ForecastDays != null)
            {
                foreach (var day in ForecastData.Forecast.ForecastDays)
                {
                    ForecastDays.Add(day);
                }
            }
        }

        private void ExecuteGoBack(object parameter)
        {

        }

        private void UpdateChart()
        {
            if (SelectedDay?.Hours == null || SelectedDay.Hours.Count == 0)
            {
                ChartModel = new PlotModel { Title = "No data available" };
                return;
            }

            var model = new PlotModel
            {
                Title = $"Forecast {SelectedChartType} - {SelectedDay.Date}",
                TitleFontSize = 16,
                PlotAreaBackground = OxyColors.White,
                Background = OxyColors.Transparent
            };

            // Filter hours - show every 3 hours for better display (8 slots per day)
            var filteredHours = SelectedDay.Hours.Where((h, i) => i % 3 == 0).ToList();
            if (filteredHours.Count == 0)
                filteredHours = SelectedDay.Hours;

            var lineSeries = new LineSeries
            {
                Color = OxyColor.FromRgb(25, 118, 210), // #1976D2
                StrokeThickness = 3,
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColor.FromRgb(25, 118, 210),
                Title = SelectedChartType
            };

            switch (SelectedChartType)
            {
                case "Temperature":
                    foreach (var hour in filteredHours)
                    {
                        lineSeries.Points.Add(new DataPoint(
                            DateTimeAxis.ToDouble(DateTime.Parse(hour.Time)),
                            hour.TempC
                        ));
                    }
                    model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm" });
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Temperature (°C)" });
                    break;

                case "Precipitation":
                    foreach (var hour in filteredHours)
                    {
                        lineSeries.Points.Add(new DataPoint(
                            DateTimeAxis.ToDouble(DateTime.Parse(hour.Time)),
                            hour.ChanceOfRain
                        ));
                    }
                    model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm" });
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Chance of Rain (%)", Minimum = 0, Maximum = 100 });
                    break;

                case "Wind":
                    foreach (var hour in filteredHours)
                    {
                        lineSeries.Points.Add(new DataPoint(
                            DateTimeAxis.ToDouble(DateTime.Parse(hour.Time)),
                            hour.WindKph
                        ));
                    }
                    model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "HH:mm" });
                    model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Wind Speed (kph)" });
                    break;
            }

            model.Series.Add(lineSeries);
            ChartModel = model;
        }
    }
}
