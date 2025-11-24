using CobaHW7.Class;
using CobaHW7.Services; // Lokasi SupabaseService
using CobaHW7.ViewModels; // Lokasi RelayCommand
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CobaHW7.ViewModels
{
    public class DashboardAdminViewModel : BaseViewModel
    {
        // 1. Properti untuk filter
        private string _boatFilter = "";
        public string BoatFilter
        {
            get => _boatFilter;
            set
            {
                _boatFilter = value;
                OnPropertyChanged(nameof(BoatFilter));
                BoatsView.Refresh(); // Refresh filter setiap kali teks berubah
            }
        }

        // 2. Properti untuk DataGrid
        private ObservableCollection<Boat> _allBoats;
        public ICollectionView BoatsView { get; private set; }
        public ObservableCollection<Booking> Bookings { get; private set; }


        // 3. Command untuk Tambah Kapal
        public ICommand AddBoatCommand { get; }
        public ICommand EditBoatCommand { get; }
        public ICommand DeleteBoatCommand { get; }


        // --- [BARU] 4. Properti & Command untuk Image Viewer ---
        private string? _largeImageUrl;
        public string? LargeImageUrl
        {
            get => _largeImageUrl;
            set
            {
                if (_largeImageUrl != value)
                {
                    _largeImageUrl = value;
                    OnPropertyChanged(nameof(LargeImageUrl));
                }
            }
        }

        public ICommand ShowImageCommand { get; }
        public ICommand HideImageCommand { get; }


        // 5. Constructor
        public DashboardAdminViewModel()
        {
            // Inisialisasi koleksi
            _allBoats = new ObservableCollection<Boat>();
            Bookings = new ObservableCollection<Booking>();

            // Buat ICollectionView
            BoatsView = CollectionViewSource.GetDefaultView(_allBoats);
            BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.ID), ListSortDirection.Ascending));
            BoatsView.Filter = FilterBoats;

            // Inisialisasi Command Lama
            AddBoatCommand = new RelayCommand(ExecuteAddBoat);
            EditBoatCommand = new RelayCommand(ExecuteEditBoat);
            DeleteBoatCommand = new RelayCommand(ExecuteDeleteBoat);

            // --- [BARU] Inisialisasi Command Image Viewer ---
            ShowImageCommand = new RelayCommand(ExecuteShowImage);
            HideImageCommand = new RelayCommand(ExecuteHideImage);

            // Muat data
            LoadDataAsync();
        }


        // --- [BARU] Metode untuk Image Viewer ---

        private void ExecuteShowImage(object parameter)
        {
            // Parameter dikirim dari XAML (CommandParameter="{Binding ThumbnailPath}")
            if (parameter is string url && !string.IsNullOrEmpty(url))
            {
                LargeImageUrl = url;
            }
        }

        private void ExecuteHideImage(object parameter)
        {
            // Set null untuk menyembunyikan pop-up (karena trigger di XAML)
            LargeImageUrl = null;
        }


        // --- Metode Lama (Tambah Kapal) ---

        private async void ExecuteAddBoat(object parameter)
        {
            AddBoatWindow addWindow = new AddBoatWindow();
            addWindow.Owner = Application.Current.MainWindow;

            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                Boat? boatFromForm = addWindow.NewBoat; // Menggunakan 'Boat?' agar aman

                if (boatFromForm == null) return;

                try
                {
                    // Simpan ke Supabase
                    Boat boatFromDb = await SupabaseService.AddBoatAsync(boatFromForm);

                    if (boatFromDb != null)
                    {
                        _allBoats.Add(boatFromDb);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Gagal menyimpan kapal baru: {ex.Message}");
                    MessageBox.Show("Gagal menyimpan data ke database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExecuteEditBoat(object parameter)
        {
            if (parameter is Boat boatToEdit)
            {
                // Buka window dengan Constructor Edit (mengirim data kapal)
                AddBoatWindow editWindow = new AddBoatWindow(boatToEdit);
                editWindow.Owner = Application.Current.MainWindow;

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    Boat? updatedBoat = editWindow.NewBoat;
                    if (updatedBoat == null) return;

                    try
                    {
                        // Panggil Service Update
                        Boat resultBoat = await SupabaseService.UpdateBoatAsync(updatedBoat);

                        if (resultBoat != null)
                        {
                            // Update tampilan di DataGrid tanpa reload database
                            // Kita cari index data lama dan ganti dengan data baru
                            var index = _allBoats.IndexOf(boatToEdit);
                            if (index != -1)
                            {
                                _allBoats[index] = resultBoat;
                            }

                            MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Gagal update: {ex.Message}");
                        MessageBox.Show("Gagal update data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void ExecuteDeleteBoat(object parameter)
        {
            // Parameter dikirim dari tombol (Objek Boat yang mau dihapus)
            if (parameter is Boat boatToDelete)
            {
                // A. Tanya Konfirmasi User dulu
                var result = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus kapal '{boatToDelete.Name}'?\nData yang dihapus tidak dapat dikembalikan.",
                    "Konfirmasi Hapus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // B. Hapus dari Database Supabase
                        await SupabaseService.DeleteBoatAsync(boatToDelete.ID);

                        // C. Hapus dari Tampilan Aplikasi (ObservableCollection)
                        // Ini penting agar barisnya hilang tanpa perlu restart aplikasi
                        _allBoats.Remove(boatToDelete);

                        MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        // Tips: Ini biasanya gagal jika Kapal sudah punya data Booking (Foreign Key Error)
                        MessageBox.Show($"Gagal menghapus data. Pastikan kapal tidak memiliki riwayat booking.\nError: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // --- Metode Lama (Load Data & Filter) ---

        private async void LoadDataAsync()
        {
            try
            {
                var boats = await SupabaseService.GetBoatsAsync();
                _allBoats.Clear();
                foreach (var boat in boats)
                {
                    _allBoats.Add(boat);
                }

                var bookings = await SupabaseService.GetBookingsAsync();
                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    Bookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Gagal memuat data: {ex.Message}");
            }
        }

        private bool FilterBoats(object item)
        {
            if (string.IsNullOrEmpty(BoatFilter)) return true;

            if (item is Boat boat)
            {
                // Cek null pada properti string untuk menghindari crash
                string name = boat.Name ?? "";
                string location = boat.Location ?? "";

                return name.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                       location.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }
    }
}
