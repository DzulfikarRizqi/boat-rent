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

        // Koleksi kapal original (sebelum filter)
        private List<Boat> AllBoats { get; set; }

        // Command saat tombol "Pilih Kapal Ini" diklik
        public ICommand SelectBoatCommand { get; }

        // Weather properties
        public ICommand SearchWeatherCommand { get; }
        public ICommand OpenForecastCommand { get; }

        // Filter & Search commands
        public ICommand ResetFilterCommand { get; }

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

        // Filter properties
        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilter();
                }
            }
        }

        private string _selectedCapacity = "Semua";
        public string SelectedCapacity
        {
            get => _selectedCapacity;
            set
            {
                if (_selectedCapacity != value)
                {
                    _selectedCapacity = value;
                    OnPropertyChanged(nameof(SelectedCapacity));
                    ApplyFilter();
                }
            }
        }

        private string _selectedPrice = "Semua";
        public string SelectedPrice
        {
            get => _selectedPrice;
            set
            {
                if (_selectedPrice != value)
                {
                    _selectedPrice = value;
                    OnPropertyChanged(nameof(SelectedPrice));
                    ApplyFilter();
                }
            }
        }

        private string _selectedLocation = "Semua";
        public string SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (_selectedLocation != value)
                {
                    _selectedLocation = value;
                    OnPropertyChanged(nameof(SelectedLocation));
                    ApplyFilter();
                }
            }
        }

        private ObservableCollection<string> _locations;
        public ObservableCollection<string> Locations
        {
            get => _locations;
            set
            {
                if (_locations != value)
                {
                    _locations = value;
                    OnPropertyChanged(nameof(Locations));
                }
            }
        }

        public DashboardUserViewModel()
        {
            Boats = new ObservableCollection<Boat>();
            AllBoats = new List<Boat>();
            Locations = new ObservableCollection<string>();
            SelectBoatCommand = new RelayCommand(ExecuteSelectBoat);
            SearchWeatherCommand = new RelayCommand(ExecuteSearchWeather);
            OpenForecastCommand = new RelayCommand(ExecuteOpenForecast);
            ResetFilterCommand = new RelayCommand(ExecuteResetFilter);

            LoadBoats();
        }

        private async void LoadBoats()
        {
            try
            {
                // Ambil semua data kapal dari Supabase
                var boatList = await SupabaseService.GetBoatsAsync();

                // Normalize location data - trim whitespace
                foreach (var boat in boatList)
                {
                    if (!string.IsNullOrWhiteSpace(boat.Location))
                    {
                        boat.Location = boat.Location.Trim();
                    }
                }

                AllBoats = boatList.ToList();

                // Build locations list dari data kapal
                var uniqueLocations = boatList.Where(b => !string.IsNullOrWhiteSpace(b.Location))
                                               .Select(b => b.Location.Trim())
                                               .Distinct(StringComparer.OrdinalIgnoreCase)
                                               .OrderBy(l => l)
                                               .ToList();

                Locations.Clear();
                Locations.Add("Semua");
                foreach (var location in uniqueLocations)
                {
                    Locations.Add(location);
                }

                // Apply initial filter
                ApplyFilter();
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
                var dashboardWindow = System.Windows.Application.Current.Windows.OfType<DashboardUser>().FirstOrDefault();
                var forecastWindow = new ForecastWindow(LocationInput, dashboardWindow);
                forecastWindow.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DashboardUserViewModel] Error opening forecast: {ex.Message}");
                MessageBox.Show($"Gagal membuka forecast: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            var filtered = AllBoats.AsEnumerable();

            // Filter by search text (nama kapal)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(b => b.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by capacity
            if (SelectedCapacity != "Semua")
            {
                filtered = filtered.Where(b =>
                {
                    return SelectedCapacity switch
                    {
                        "2-10 Orang" => b.Capacity >= 2 && b.Capacity <= 10,
                        "11-30 Orang" => b.Capacity >= 11 && b.Capacity <= 30,
                        "30-100 Orang" => b.Capacity > 30 && b.Capacity <= 100,
                        "100+ Orang" => b.Capacity > 100,
                        _ => true
                    };
                });
            }

            // Filter by price
            if (SelectedPrice != "Semua")
            {
                filtered = filtered.Where(b =>
                {
                    return SelectedPrice switch
                    {
                        "Rp 0 - 10JT" => b.PricePerDay <= 10000000,
                        "Rp 10JT - 30JT" => b.PricePerDay > 10000000 && b.PricePerDay <= 30000000,
                        "Rp 30JT - 50JT" => b.PricePerDay > 30000000 && b.PricePerDay <= 50000000,
                        "Rp 50JT - 100JT" => b.PricePerDay > 50000000 && b.PricePerDay <= 100000000,
                        "Rp 100JT+" => b.PricePerDay > 100000000,
                        _ => true
                    };
                });
            }

            // Filter by location
            if (SelectedLocation != "Semua")
            {
                filtered = filtered.Where(b => !string.IsNullOrWhiteSpace(b.Location) &&
                                               b.Location.Trim().Equals(SelectedLocation.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            // Update UI
            Boats.Clear();
            foreach (var boat in filtered)
            {
                Boats.Add(boat);
            }
        }

        private void ExecuteResetFilter(object parameter)
        {
            SearchText = "";
            SelectedCapacity = "Semua";
            SelectedPrice = "Semua";
            SelectedLocation = "Semua";
        }
    }
}