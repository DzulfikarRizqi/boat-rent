using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using CobaHW7.Data;
=======
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CobaHW7.Data;
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class Dashboard : Window
    {
        private readonly User _currentUser;

<<<<<<< HEAD
=======
        // ---- Booking list state ----
        private readonly BookingRepository _bookRepo = new();
        private readonly ObservableCollection<BookingRow> _bookings = new();
        private ICollectionView? _bookingView;

>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
        public Dashboard(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
<<<<<<< HEAD
            Loaded += Dashboard_Loaded;
        }

        private async void Dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBoatsFromDb();
        }

        private async Task LoadBoatsFromDb()
        {
            var repo = new BoatRepository();
            List<Boat> boats = await repo.GetAllAsync();

            BoatPanel.Children.Clear();

            foreach (var boat in boats)
            {
                var card = new Border
                {
                    Width = 250,
                    Height = 340,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(10),
                    BorderThickness = new Thickness(1),
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    Background = System.Windows.Media.Brushes.White
                };

                var stack = new StackPanel { Margin = new Thickness(10) };

                var image = new Image
                {
                    Source = string.IsNullOrWhiteSpace(boat.Image)
                        ? null
                        : new BitmapImage(new Uri(boat.Image, UriKind.RelativeOrAbsolute)),
                    Height = 150,
                    Stretch = System.Windows.Media.Stretch.UniformToFill,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var title = new TextBlock
                {
                    Text = boat.Model,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var info = new TextBlock
                {
                    Text = $"Harga: Rp {boat.Price:N0} / hari\nStatus: {(boat.Available ? "Tersedia" : "Tidak Tersedia")}",
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var button = new Button
                {
                    Content = "Pesan Kapal Ini",
                    Background = boat.Available
                        ? System.Windows.Media.Brushes.SteelBlue
                        : System.Windows.Media.Brushes.Gray,
                    Foreground = System.Windows.Media.Brushes.White,
                    IsEnabled = boat.Available,
                    Padding = new Thickness(5),
                    Tag = boat.ID,
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                button.Click += async (s, e) => await PesanBoat(boat);

                stack.Children.Add(image);
                stack.Children.Add(title);
                stack.Children.Add(info);
                stack.Children.Add(button);
                card.Child = stack;
                BoatPanel.Children.Add(card);
=======

            // hook setelah komponen siap
            Loaded += Dashboard_Loaded;
        }

        // ============================================================
        // LIFECYCLE
        // ============================================================
        private async void Dashboard_Loaded(object? sender, RoutedEventArgs e)
        {
            // siapkan binding untuk kartu booking
            BookingList.ItemsSource = _bookings;
            _bookingView = CollectionViewSource.GetDefaultView(BookingList.ItemsSource);
            _bookingView.Filter = BookingFilter;

            // load kapal
            await LoadBoatsFromDb();
        }

        // ============================================================
        // KAPAL: render kartu dari DB
        // ============================================================
        private async Task LoadBoatsFromDb()
        {
            try
            {
                var repo = new BoatRepository();
                List<Boat> boats = await repo.GetAllAsync();

                BoatPanel.Children.Clear();

                foreach (var boat in boats)
                {
                    var card = new Border
                    {
                        Width = 260,
                        Height = 360,
                        Margin = new Thickness(10),
                        CornerRadius = new CornerRadius(12),
                        BorderThickness = new Thickness(1),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                        Background = Brushes.White,
                        Effect = new System.Windows.Media.Effects.DropShadowEffect
                        {
                            Color = Color.FromArgb(0x22, 0, 0, 0),
                            BlurRadius = 12,
                            ShadowDepth = 0
                        }
                    };

                    var stack = new StackPanel { Margin = new Thickness(10) };

                    var image = new Image
                    {
                        Source = string.IsNullOrWhiteSpace(boat.Image)
                            ? null
                            : new BitmapImage(new Uri(boat.Image, UriKind.RelativeOrAbsolute)),
                        Height = 150,
                        Stretch = Stretch.UniformToFill,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    var title = new TextBlock
                    {
                        Text = string.IsNullOrWhiteSpace(boat.Model) ? boat.Name : boat.Model,
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    var info = new TextBlock
                    {
                        Text = $"Harga: Rp {boat.Price:N0} / hari\nStatus: {(boat.Available ? "Tersedia" : "Tidak Tersedia")}",
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 13,
                        Foreground = Brushes.Gray,
                        TextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10)
                    };

                    var button = new Button
                    {
                        Content = "Pesan Kapal Ini",
                        Background = boat.Available ? new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)) : Brushes.Gray,
                        Foreground = Brushes.White,
                        IsEnabled = boat.Available,
                        Padding = new Thickness(6),
                        Tag = boat.ID,
                        Width = 160,
                        Height = 32,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Cursor = System.Windows.Input.Cursors.Hand
                    };
                    button.Click += async (s, e) => await PesanBoat(boat);

                    stack.Children.Add(image);
                    stack.Children.Add(title);
                    stack.Children.Add(info);
                    stack.Children.Add(button);
                    card.Child = stack;

                    BoatPanel.Children.Add(card);
                }
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal memuat kapal", ex.Message);
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
            }
        }

        private async Task PesanBoat(Boat boat)
        {
<<<<<<< HEAD
            // contoh sederhana: pesan untuk hari ini 1 hari
            var b = new Booking
            {
                //UserId = _currentUser.Id,
                BoatId = boat.ID,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                TotalAmount = (decimal)boat.Price,
                Status = BookingStatus.Pending
            };

            var repo = new BookingRepository();
            await repo.CreateAsync(b);

            MessageBox.Show("Pesanan tersimpan di database.");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
        // === Added handlers to satisfy XAML ===
        private void SelectBoat_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var tag = btn.Tag?.ToString() ?? "?";
                MessageBox.Show($"Kapal dipilih (Tag={tag}). Lanjutkan logika pemesanan Anda.");
            }
            else
            {
                MessageBox.Show("Kapal dipilih. Tambahkan logika sesuai kebutuhan.");
            }
        }

        private void CekBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new BookingWindow();
                win.Owner = this;
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka BookingWindow. {ex.Message}");
=======
            try
            {
                // contoh sederhana: pesan untuk hari ini 1 hari
                var b = new Booking
                {
                    UserId = _currentUser.Id,
                    BoatId = boat.ID,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(1),
                    TotalAmount = (decimal)boat.Price,
                    Status = BookingStatus.Pending
                };

                // Asumsikan kamu sudah punya CreateAsync di BookingRepository
                await _bookRepo.CreateAsync(b);

                DialogService.Success("Berhasil", "Pesanan tersimpan. Kamu bisa melihatnya di Cek Booking.");

                // refresh list booking bila panel sedang terbuka
                if (BookingSection.Visibility == Visibility.Visible)
                    await LoadBookingsAsync();
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal memesan", ex.Message);
            }
        }

        // ============================================================
        // BOOKING: load, filter, UI handlers
        // ============================================================
        private async Task LoadBookingsAsync()
        {
            try
            {
                _bookings.Clear();
                var rows = await _bookRepo.GetByUserAsync(_currentUser.Id);
                foreach (var r in rows) _bookings.Add(r);
                _bookingView?.Refresh();
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal memuat booking", ex.Message);
            }
        }

        private bool BookingFilter(object obj)
        {
            if (obj is not BookingRow r) return false;

            var q = (SearchBox?.Text ?? "").Trim().ToLowerInvariant();
            var status = (StatusFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Semua";

            bool matchStatus = status == "Semua" ||
                               string.Equals(r.Status, status, StringComparison.OrdinalIgnoreCase);

            bool matchQuery = string.IsNullOrEmpty(q) ||
                              (r.BoatName?.ToLowerInvariant().Contains(q) ?? false);

            return matchStatus && matchQuery;
        }

        private async void RefreshBookings_Click(object sender, RoutedEventArgs e)
        {
            await LoadBookingsAsync();
        }

        private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _bookingView?.Refresh();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _bookingView?.Refresh();
        }

        private void OpenBookingDetail_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is not BookingRow row) return;

            var idId = new CultureInfo("id-ID");
            var total = string.Format(idId, "Rp {0:N0}", row.TotalAmount);
            DialogService.Info("Detail Booking",
                $"ID: {row.Id}\nKapal: {row.BoatName}\nTanggal: {row.StartDate:dd MMM yyyy} — {row.EndDate:dd MMM yyyy}\nStatus: {row.Status}\nTotal: {total}");
        }

        // tombol di header
        private async void CekBooking_Click(object sender, RoutedEventArgs e)
        {
            // tampilkan panel booking, sembunyikan daftar kapal
            BookingSection.Visibility = Visibility.Visible;
            BoatScroll.Visibility = Visibility.Collapsed;

            await LoadBookingsAsync();
        }

        // (opsional) panggil ini jika kamu menambahkan tombol "Kembali ke Kapal"
        private void ShowBoats()
        {
            BookingSection.Visibility = Visibility.Collapsed;
            BoatScroll.Visibility = Visibility.Visible;
        }

        // ============================================================
        // MISC (logout, dll)
        // ============================================================
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = new MainWindow();
                Application.Current.MainWindow = login;
                login.Show();
                Close();
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal logout", ex.Message);
            }
        }

        // ======== Legacy handlers (kalau masih dipanggil dari XAML lama) ========
        private void SelectBoat_Click(object sender, RoutedEventArgs e)
        {
            // Tidak digunakan pada versi dynamic-boat; dipertahankan untuk kompatibilitas
            if (sender is Button btn)
            {
                MessageBox.Show($"Kapal dipilih (Tag={btn.Tag}).");
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
            }
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Book Now diklik. Implementasikan penyimpanan ke database di sini.");
        }
<<<<<<< HEAD

=======
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
    }
}
