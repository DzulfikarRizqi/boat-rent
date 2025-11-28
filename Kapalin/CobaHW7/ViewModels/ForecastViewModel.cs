using CobaHW7.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ForecastViewModel()
        {
            ForecastDays = new ObservableCollection<ForecastDay>();
            GoBackCommand = new RelayCommand(ExecuteGoBack);
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
    }
}
