using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CobaHW7.ViewModels;
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class Dashboard : Window
    {
        private bool isSidebarVisible = false;
        public Dashboard()
        {
            InitializeComponent();
            this.DataContext = new DashboardUserViewModel();
        }

        private async void BtnCekBooking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable button
                var button = sender as Button;
                if (button != null)
                    button.IsEnabled = false;

                // Ambil user yang sedang login
                var currentUser = SupabaseService.Client.Auth.CurrentUser;
                if (currentUser == null)
                {
                    var alert = new AlertWindow("Error", "User tidak ditemukan. Silakan login kembali.", AlertWindow.AlertType.Error);
                    alert.ShowDialog();
                    if (button != null) button.IsEnabled = true;
                    return;
                }

                // Ambil data user dari tabel users untuk mendapatkan ID
                var allUsers = await SupabaseService.Client
                    .From<User>()
                    .Get();

                var user = allUsers.Models.FirstOrDefault(u =>
                    u.Email != null && u.Email.Equals(currentUser.Email, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    var alert = new AlertWindow("Error", "Data user tidak ditemukan.", AlertWindow.AlertType.Error);
                    alert.ShowDialog();
                    if (button != null) button.IsEnabled = true;
                    return;
                }

                // Ambil booking dari user ini
                var bookings = await SupabaseService.Client
                    .From<Booking>()
                    .Where(b => b.UserId == user.Id)
                    .Get();

                if (bookings.Models.Count == 0)
                {
                    var alert = new AlertWindow("Booking Kosong", "Anda belum memiliki booking.", AlertWindow.AlertType.Info);
                    alert.ShowDialog();
                    if (button != null) button.IsEnabled = true;
                    return;
                }

                // Buka window untuk menampilkan booking
                BookingWindow bookingWindow = new BookingWindow(bookings.Models);
                bookingWindow.ShowDialog();

                if (button != null) button.IsEnabled = true;
            }
            catch (Exception ex)
            {
                var alert = new AlertWindow("Error", $"Terjadi kesalahan: {ex.Message}", AlertWindow.AlertType.Error);
                alert.ShowDialog();

                var button = sender as Button;
                if (button != null) button.IsEnabled = true;
            }
        }
        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Tutup sidebar dengan animasi halus
            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = -220,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new System.Windows.Media.Animation.QuadraticEase()
            };

            SidebarTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);

            // Sembunyikan overlay setelah sidebar tertutup
            Overlay.Visibility = Visibility.Collapsed;
            isSidebarVisible = false;
        }
        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            double from = isSidebarVisible ? 0 : -220;
            double to = isSidebarVisible ? -220 : 0;

            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new System.Windows.Media.Animation.QuadraticEase()
            };

            SidebarTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, animation);

            // Tampilkan / sembunyikan overlay
            if (!isSidebarVisible)
            {
                Overlay.Visibility = Visibility.Visible; // tampilkan overlay
            }
            else
            {
                Overlay.Visibility = Visibility.Collapsed; // sembunyikan overlay
            }

            isSidebarVisible = !isSidebarVisible;
        }
        private void AboutUsSection_Click(object sender, RoutedEventArgs e)
        {
            MainScroll.ScrollToVerticalOffset(
                AboutUsPanel.TransformToVisual(MainScroll).Transform(new Point(0, 0)).Y);

            ToggleSidebar_Click(sender, e);
        }

        private void WhyUsSection_Click(object sender, RoutedEventArgs e)
        {
            MainScroll.ScrollToVerticalOffset(
                WhyUsPanel.TransformToVisual(MainScroll).Transform(new Point(0, 0)).Y);

            ToggleSidebar_Click(sender, e);
        }

        private void RentalSection_Click(object sender, RoutedEventArgs e)
        {
            MainScroll.ScrollToVerticalOffset(BoatPanel.TransformToVisual(MainScroll)
                .Transform(new Point(0, 0)).Y);

            // Tutup sidebar setelah klik
            ToggleSidebar_Click(sender, e);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // Disable button to prevent multiple clicks
                var button = sender as Button;
                if (button != null)
                    button.IsEnabled = false;


                try
                {
                    var signOutTask = SupabaseService.Client.Auth.SignOut();
                }
                catch
                {
                }


                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                var errorAlert = new AlertWindow("Logout",
                    "Anda akan diarahkan ke halaman login.",
                    AlertWindow.AlertType.Info);
                errorAlert.ShowDialog();

                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
