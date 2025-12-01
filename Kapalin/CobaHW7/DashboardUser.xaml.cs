using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using CobaHW7.ViewModels;
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class DashboardUser : Window
    {
        private bool _isSidebarOpen = false;

        public DashboardUser()
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

        private void AboutUsSection_Click(object sender, RoutedEventArgs e)
        {
            AboutUsPanel?.BringIntoView();
            CloseSidebar();
        }

        private void WhyUsSection_Click(object sender, RoutedEventArgs e)
        {
            WhyUsPanel?.BringIntoView();
            CloseSidebar();
        }

        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            if (_isSidebarOpen)
                CloseSidebar();
            else
                OpenSidebar();
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseSidebar();
        }

        private void OpenSidebar()
        {
            _isSidebarOpen = true;
            Overlay.Visibility = Visibility.Visible;

            var anim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            SidebarTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, anim);
        }

        private void CloseSidebar()
        {
            _isSidebarOpen = false;
            Overlay.Visibility = Visibility.Collapsed;

            var anim = new DoubleAnimation
            {
                To = -Sidebar.ActualWidth,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            SidebarTransform.BeginAnimation(System.Windows.Media.TranslateTransform.XProperty, anim);
        }

        private void RentalSection_Click(object sender, RoutedEventArgs e)
        {
            FilterSection?.BringIntoView();
            CloseSidebar();
        }

        private void WeatherSection_Click(object sender, RoutedEventArgs e)
        {
            WeatherSection?.BringIntoView();
            CloseSidebar();
        }
    }
}
