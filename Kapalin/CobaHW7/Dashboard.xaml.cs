using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using CobaHW7.Data;
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class Dashboard : Window
    {
        private readonly User _currentUser;

        public Dashboard(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
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
            }
        }

        private async Task PesanBoat(Boat boat)
        {
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
            }
        }

        private void BookNow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Book Now diklik. Implementasikan penyimpanan ke database di sini.");
        }

    }
}
