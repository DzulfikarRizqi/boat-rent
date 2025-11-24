using CobaHW7.Class;
using CobaHW7.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace CobaHW7.ViewModels
{
    public class DashboardUserViewModel : BaseViewModel
    {
        // Koleksi kapal untuk ditampilkan di UI
        public ObservableCollection<Boat> Boats { get; private set; }

        // Command saat tombol "Pilih Kapal Ini" diklik
        public ICommand SelectBoatCommand { get; }

        public DashboardUserViewModel()
        {
            Boats = new ObservableCollection<Boat>();
            SelectBoatCommand = new RelayCommand(ExecuteSelectBoat);

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
                    // Opsional: Hanya tampilkan kapal yang aktif? 
                    // Jika iya, uncomment baris bawah ini:
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

                // Buka RentalBookingWindow
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
    }
}