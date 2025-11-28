using CobaHW7.Class;
using CobaHW7.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace CobaHW7.ViewModels
{
    public class DashboardUserViewModel : BaseViewModel
    {
        // Koleksi kapal untuk ditampilkan di UI
        public ObservableCollection<Boat> Boats { get; private set; }

        // Command saat tombol "Pilih Kapal Ini" diklik
        public ICommand SelectBoatCommand { get; }

        // Weather properties
        public ICommand SearchWeatherCommand { get; }
        public ICommand OpenForecastCommand { get; }

        private string _locationInput;
        public string LocationInput
        {
            get => _locationInput;
            set
            {
                if (_locationInput != value)
                {
                    _locationInput = value;
                    OnPropertyChanged(nameof(LocationInput));
                }
            }
        }

        private WeatherData _currentWeather;
        public WeatherData CurrentWeather
        {
            get => _currentWeather;
            set
            {
                if (_currentWeather != value)
                {
                    _currentWeather = value;
                    OnPropertyChanged(nameof(CurrentWeather));
                }
            }
        }

        private bool _isLoadingWeather;
        public bool IsLoadingWeather
        {
            get => _isLoadingWeather;
            set
            {
                if (_isLoadingWeather != value)
                {
                    _isLoadingWeather = value;
                    OnPropertyChanged(nameof(IsLoadingWeather));
                }
            }
        }

        private string _weatherErrorMessage;
        public string WeatherErrorMessage
        {
            get => _weatherErrorMessage;
            set
            {
                if (_weatherErrorMessage != value)
                {
                    _weatherErrorMessage = value;
                    OnPropertyChanged(nameof(WeatherErrorMessage));
                }
            }
        }

        private bool _hasWeatherError;
        public bool HasWeatherError
        {
            get => _hasWeatherError;
            set
            {
                if (_hasWeatherError != value)
                {
                    _hasWeatherError = value;
                    OnPropertyChanged(nameof(HasWeatherError));
                }
            }
        }

        public DashboardUserViewModel()
        {
            Boats = new ObservableCollection<Boat>();
            SelectBoatCommand = new RelayCommand(ExecuteSelectBoat);
            SearchWeatherCommand = new RelayCommand(ExecuteSearchWeather);
            OpenForecastCommand = new RelayCommand(ExecuteOpenForecast);

            LoadBoats();
        }

        private async void LoadBoats()
        {
            try
            {
                // Ambil semua data kapal dari Supabase
                var boatList = await SupabaseService.GetBoatsAsync();

                Boats.Clear();
                foreach (var boat in boatList)
                {
                    // if (!boat.Available) continue; 
                    Boats.Add(boat);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading boats: {ex.Message}");
                MessageBox.Show("Gagal memuat daftar kapal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSelectBoat(object parameter)
        {
            if (parameter is Boat selectedBoat)
            {
                if (selectedBoat.Available != true)
                {
                    MessageBox.Show("Maaf, kapal ini sedang tidak tersedia / dalam perbaikan.", "Tidak Tersedia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var rentalWindow = new RentalBookingWindow(selectedBoat);
                    rentalWindow.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membuka halaman booking: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExecuteSearchWeather(object parameter)
        {
            if (string.IsNullOrWhiteSpace(LocationInput))
            {
                HasWeatherError = true;
                WeatherErrorMessage = "Silakan masukkan lokasi terlebih dahulu";
                return;
            }

            try
            {
                IsLoadingWeather = true;
                HasWeatherError = false;
                WeatherErrorMessage = "";

                var weatherData = await WeatherService.GetWeatherAsync(LocationInput);

                if (weatherData == null)
                {
                    HasWeatherError = true;
                    WeatherErrorMessage = $"Lokasi '{LocationInput}' tidak ditemukan. Coba nama kota lain seperti Bali, Jakarta, atau Lombok.";
                    CurrentWeather = null;
                }
                else
                {
                    CurrentWeather = weatherData;
                    HasWeatherError = false;
                    Debug.WriteLine($"[DashboardUserViewModel] Weather loaded: {weatherData.Location.Name}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DashboardUserViewModel] Error fetching weather: {ex.Message}");
                HasWeatherError = true;
                WeatherErrorMessage = $"Gagal memuat data cuaca: {ex.Message}";
                CurrentWeather = null;
            }
            finally
            {
                IsLoadingWeather = false;
            }
        }

        private void ExecuteOpenForecast(object parameter)
        {
            if (CurrentWeather == null)
            {
                MessageBox.Show("Silakan cari cuaca lokasi terlebih dahulu.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var dashboardWindow = System.Windows.Application.Current.Windows.OfType<Dashboard>().FirstOrDefault();
                var forecastWindow = new ForecastWindow(LocationInput, dashboardWindow);
                forecastWindow.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DashboardUserViewModel] Error opening forecast: {ex.Message}");
                MessageBox.Show($"Gagal membuka forecast: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}