using CobaHW7.Class;
using CobaHW7.Supabase; // lokasi SupabaseService
using CobaHW7.ViewModels; // <-- TAMBAHKAN INI (jika RelayCommand di sana)
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;  // <-- TAMBAHKAN INI
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input; // <-- TAMBAHKAN INI

namespace CobaHW7.ViewModels
{
    //public class DashboardAdminViewModel : BaseViewModel
    //{

    //    // 2. Properti untuk filter (sesuai binding di XAML )
    //    private string _boatFilter;
    //    public string BoatFilter
    //    {
    //        get => _boatFilter;
    //        set
    //        {
    //            _boatFilter = value;
    //            OnPropertyChanged(nameof(BoatFilter));
    //            // Refresh filter setiap kali teks berubah
    //            BoatsView.Refresh();
    //        }
    //    }

    //    // 3. Properti untuk DataGrid (sesuai binding di XAML )

    //    // Ini adalah koleksi *internal* untuk menyimpan SEMUA kapal
    //    private ObservableCollection<Boat> _allBoats;

    //    // Ini adalah 'View' yang akan dilihat oleh DataGrid 
    //    // ICollectionView sangat bagus karena bisa difilter
    //    public ICollectionView BoatsView { get; private set; }

    //    // Properti untuk data booking 
    //    public ObservableCollection<Booking> Bookings { get; private set; }

    //    public DashboardAdminViewModel()
    //    {

    //        // Inisialisasi koleksi
    //        _allBoats = new ObservableCollection<Boat>();
    //        Bookings = new ObservableCollection<Booking>();

    //        // Buat ICollectionView berdasarkan koleksi internal _allBoats
    //        BoatsView = CollectionViewSource.GetDefaultView(_allBoats);

    //        // Terapkan logika filter
    //        BoatsView.Filter = FilterBoats;

    //        // Panggil metode untuk memuat data
    //        LoadDataAsync();
    //    }

    //    // 5. Metode untuk memuat data dari Supabase
    //    private async void LoadDataAsync()
    //    {
    //        try
    //        {
    //            // Ambil data kapal
    //            var boats = await SupabaseService.GetBoatsAsync();
    //            _allBoats.Clear();
    //            foreach (var boat in boats)
    //            {
    //                _allBoats.Add(boat);
    //            }

    //            // Ambil data booking
    //            var bookings = await SupabaseService.GetBookingsAsync();
    //            Bookings.Clear();
    //            foreach (var booking in bookings)
    //            {
    //                Bookings.Add(booking);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Tangani error, misalnya tampilkan MessageBox
    //            Console.WriteLine($"Gagal memuat data: {ex.Message}");
    //        }
    //    }

    //    // 6. Logika untuk memfilter DataGrid kapal
    //    private bool FilterBoats(object item)
    //    {
    //        if (string.IsNullOrEmpty(BoatFilter))
    //        {
    //            // Jika filter kosong, tampilkan semua
    //            return true;
    //        }

    //        if (item is Boat boat)
    //        {
    //            // Cek apakah nama kapal mengandung teks filter (tidak case-sensitive)
    //            return boat.Name.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
    //                   boat.Location.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0;
    //        }

    //        return false;
    //    }
    //}

    public class DashboardAdminViewModel : BaseViewModel
    {
        // 2. Properti untuk filter (sesuai binding di XAML )
        // Saya tambahkan = "" untuk mengatasi warning nullable CS8618
        private string _boatFilter = "";
        public string BoatFilter
        {
            get => _boatFilter;
            set
            {
                _boatFilter = value;
                OnPropertyChanged(nameof(BoatFilter));
                // Refresh filter setiap kali teks berubah
                BoatsView.Refresh();
            }
        }

        // 3. Properti untuk DataGrid (sesuai binding di XAML )
        private ObservableCollection<Boat> _allBoats;
        public ICollectionView BoatsView { get; private set; }
        public ObservableCollection<Booking> Bookings { get; private set; }


        // --- PROPERTI COMMAND BARU ---
        public ICommand AddBoatCommand { get; } // <-- BARU


        // 4. Constructor
        public DashboardAdminViewModel()
        {
            // Inisialisasi koleksi
            _allBoats = new ObservableCollection<Boat>();
            Bookings = new ObservableCollection<Booking>();

            // Buat ICollectionView berdasarkan koleksi internal _allBoats
            BoatsView = CollectionViewSource.GetDefaultView(_allBoats);

            // Terapkan logika filter
            BoatsView.Filter = FilterBoats;

            // --- INISIALISASI COMMAND BARU ---
            // 'ExecuteAddBoat' adalah nama metode yang akan dipanggil
            AddBoatCommand = new RelayCommand(ExecuteAddBoat); // <-- BARU

            // Panggil metode untuk memuat data
            LoadDataAsync();
        }


        // --- METODE BARU UNTUK MENANGANI TOMBOL ---

        /// <summary>
        /// Dipanggil oleh AddBoatCommand (tombol "Tambah Kapal")
        /// </summary>
        private async void ExecuteAddBoat(object parameter) // <-- BARU (SELURUH METODE)
        {
            // Buat instance window form baru
            AddBoatWindow addWindow = new AddBoatWindow();

            // Set 'Owner' agar window muncul di tengah aplikasi utama
            addWindow.Owner = Application.Current.MainWindow;

            // Tampilkan window sebagai dialog (menghentikan eksekusi di sini)
            bool? result = addWindow.ShowDialog();

            // Cek apakah user mengklik "Simpan" (DialogResult == true)
            if (result == true)
            {
                // Ambil objek Boat baru dari properti di AddBoatWindow
                Boat boatFromForm = addWindow.NewBoat;

                try
                {
                    // Panggil service untuk menyimpan ke Supabase
                    // 'AddBoatAsync' akan mengembalikan kapal baru lengkap dengan ID
                    Boat boatFromDb = await SupabaseService.AddBoatAsync(boatFromForm);

                    if (boatFromDb != null)
                    {
                        // PENTING: Tambahkan kapal baru ke koleksi lokal
                        // Ini akan otomatis update DataGrid di UI
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


        // --- METODE LAMA ANDA (Sudah Benar) ---

        // 5. Metode untuk memuat data dari Supabase
        private async void LoadDataAsync()
        {
            try
            {
                // Ambil data kapal
                var boats = await SupabaseService.GetBoatsAsync();
                _allBoats.Clear();
                foreach (var boat in boats)
                {
                    _allBoats.Add(boat);
                }

                // Ambil data booking
                var bookings = await SupabaseService.GetBookingsAsync();
                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    Bookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                // Mengganti Console.WriteLine ke Debug.WriteLine
                Debug.WriteLine($"Gagal memuat data: {ex.Message}");
            }
        }

        // 6. Logika untuk memfilter DataGrid kapal
        private bool FilterBoats(object item)
        {
            if (string.IsNullOrEmpty(BoatFilter))
            {
                // Jika filter kosong, tampilkan semua
                return true;
            }

            if (item is Boat boat)
            {
                // Cek apakah nama kapal mengandung teks filter (tidak case-sensitive)
                return boat.Name.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                       boat.Location.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return false;
        }
    }
}
