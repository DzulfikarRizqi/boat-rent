<<<<<<< HEAD
﻿using CobaHW7.Class;
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
=======
﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CobaHW7.Class;

namespace CobaHW7.ViewModels
{
    public class DashboardAdminViewModel : BaseViewModel
    {
        // koleksi yang di-bind ke Tab Kapal & Tab Booking
        public ObservableCollection<Boat> Boats { get; }
        public ObservableCollection<Booking> Bookings { get; }

        // biar DataGrid bisa pakai CurrentItem (sesuai code-behind kamu)
        public ListCollectionView BoatsView { get; }
        public ListCollectionView BookingsView { get; }

        private int _nextBoatId = 4;
        private int _nextBookingId = 3;

        public DashboardAdminViewModel()
        {
            // seed contoh kapal
            Boats = new ObservableCollection<Boat>
            {
                new Boat { ID = 1, Name = "Speedboat 20ft", Model = "Speed 20", PricePerDay = 750_000, Available = true, Location = "Jakarta" },
                new Boat { ID = 2, Name = "Kapal Wisata",   Model = "Tour 30",  PricePerDay = 1_200_000, Available = true, Location = "Bali" },
                new Boat { ID = 3, Name = "Fishing Boat",   Model = "Fish 15",  PricePerDay = 500_000,   Available = true, Location = "Makassar" },
            };

            // seed contoh booking
            Bookings = new ObservableCollection<Booking>
            {
                new Booking
                {
                    BookingId = 1,
                    BoatId = 1,
                    UserId = 101,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(2),
                    Status = BookingStatus.Pending,
                    TotalAmount = 1_500_000
                },
                new Booking
                {
                    BookingId = 2,
                    BoatId = 2,
                    UserId = 102,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(3),
                    Status = BookingStatus.Confirmed,
                    PaymentMethod = PaymentKind.Cash,
                    TotalAmount = 2_400_000
                }
            };

            BoatsView = new ListCollectionView(Boats);
            BookingsView = new ListCollectionView(Bookings);
        }

        public void AddBoat()
        {
            var boat = new Boat
            {
                ID = _nextBoatId++,
                Name = "Boat Baru",
                Model = "Model",
                PricePerDay = 600_000,
                Available = true,
                Location = ""
            };
            Boats.Add(boat);
        }

        public void DeleteBoat(Boat boat)
        {
            if (boat == null) return;
            Boats.Remove(boat);
        }

        public void ConfirmBooking(Booking booking)
        {
            if (booking == null) return;

            // kalau TotalAmount belum diisi, isi default
            if (booking.TotalAmount <= 0)
            {
                booking.TotalAmount = 1_000_000;
            }

            booking.ConfirmPayment(PaymentKind.Cash, booking.TotalAmount);
            OnPropertyChanged(nameof(Bookings));
        }

        public void CancelBooking(Booking booking)
        {
            if (booking == null) return;

            booking.Cancel();
            OnPropertyChanged(nameof(Bookings));
        }
    }
}


//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows.Data;
//using CobaHW7.Class;

//namespace CobaHW7.ViewModels 
//{
//    public class DashboardAdminViewModel : INotifyPropertyChanged
//    {
//        public ObservableCollection<Boat> Boats { get; } = new();
//        public ICollectionView BoatsView { get; }

//        private string _boatFilter;
//        public string BoatFilter
//        {
//            get => _boatFilter;
//            set { _boatFilter = value; OnChanged(nameof(BoatFilter)); BoatsView.Refresh(); }
//        }

//        public ObservableCollection<Booking> Bookings { get; } = new();

//        public DashboardAdminViewModel()
//        {
//            Seed();
//            BoatsView = CollectionViewSource.GetDefaultView(Boats);
//            BoatsView.Filter = FilterBoat;
//        }

//        private bool FilterBoat(object obj)
//        {
//            if (obj is not Boat b) return false;
//            if (string.IsNullOrWhiteSpace(BoatFilter)) return true;
//            var t = BoatFilter.Trim().ToLowerInvariant();
//            return (b.Name ?? "").ToLowerInvariant().Contains(t)
//                || (b.Model ?? "").ToLowerInvariant().Contains(t)
//                || (b.Location ?? "").ToLowerInvariant().Contains(t);
//        }

//        public void AddBoat()
//        {
//            int newId = Boats.Any() ? Boats.Max(x => x.ID) + 1 : 1;
//            Boats.Add(new Boat
//            {
//                ID = newId,
//                Name = "Kapal Baru",
//                Model = "",
//                Location = "",
//                Capacity = 0,
//                Year = DateTime.Now.Year,
//                Rating = 0,
//                PricePerDay = 0,
//                Available = true,
//                ThumbnailPath = ""
//            });
//        }

//        public void DeleteBoat(Boat boat)
//        {
//            if (boat == null) return;
//            Boats.Remove(boat);

//            var toRemove = Bookings.Where(b => b.BoatId == boat.ID).ToList();
//            foreach (var b in toRemove) Bookings.Remove(b);
//        }

//        public void ConfirmBooking(Booking bk)
//        {
//            if (bk == null) return;

//            if (bk.TotalAmount <= 0)
//            {
//                var boat = Boats.FirstOrDefault(x => x.ID == bk.BoatId);
//                if (boat != null)
//                {
//                    var days = Math.Max(1, (bk.EndDate - bk.StartDate).Days);
//                    bk.ConfirmPayment(bk.PaymentMethod, boat.PricePerDay * days);
//                }
//                else
//                {
//                    bk.ConfirmPayment(bk.PaymentMethod, bk.TotalAmount);
//                }
//            }
//            else
//            {
//                bk.ConfirmPayment(bk.PaymentMethod, bk.TotalAmount);
//            }

//            Boats.FirstOrDefault(x => x.ID == bk.BoatId)?.AddBooking(bk.StartDate, bk.EndDate);
//            OnChanged(nameof(Bookings));
//        }

//        public void CancelBooking(Booking bk)
//        {
//            if (bk == null) return;
//            bk.Cancel();
//            OnChanged(nameof(Bookings));
//        }

//        private void Seed()
//        {
//            if (Boats.Count > 0) return;
//            Boats.Add(new Boat
//            {
//                ID = 1,
//                Name = "Samudra 1",
//                Model = "Speedboat 22",
//                Location = "Jakarta",
//                Capacity = 8,
//                Year = 2021,
//                Rating = 4.7,
//                PricePerDay = 3500000,
//                Available = true
//            });
//            Boats.Add(new Boat
//            {
//                ID = 2,
//                Name = "Pelangi Laut",
//                Model = "Yacht 35",
//                Location = "Bali",
//                Capacity = 12,
//                Year = 2019,
//                Rating = 4.9,
//                PricePerDay = 9500000,
//                Available = true
//            });
//            Boats.Add(new Boat
//            {
//                ID = 3,
//                Name = "Nusantara",
//                Model = "Fishing 28",
//                Location = "Makassar",
//                Capacity = 6,
//                Year = 2018,
//                Rating = 4.3,
//                PricePerDay = 2500000,
//                Available = true
//            });

//            Bookings.Add(new Booking(1001, 1, 101, DateTime.Today.AddDays(3), DateTime.Today.AddDays(5)));
//            Bookings.Add(new Booking(1002, 2, 102, DateTime.Today.AddDays(7), DateTime.Today.AddDays(8)));
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//    }
//}
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
