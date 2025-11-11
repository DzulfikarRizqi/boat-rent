using System;
using System.Collections.Generic;
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
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class Dashboard : Window
    {
        private readonly User _currentUser;

        // ---- Booking list state ----
        private readonly BookingRepository _bookRepo = new();
        private readonly ObservableCollection<BookingRow> _bookings = new();
        private ICollectionView? _bookingView;

        public Dashboard(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

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
            }
        }

        private async Task PesanBoat(Boat boat)
        {
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
            }
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Book Now diklik. Implementasikan penyimpanan ke database di sini.");
        }
    }
}
