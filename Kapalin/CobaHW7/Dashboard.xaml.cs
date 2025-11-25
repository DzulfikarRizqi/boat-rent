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
                long userId = long.Parse(user.Id);
                var bookings = await SupabaseService.Client
                    .From<Booking>()
                    .Where(b => b.UserId == userId)
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
    }
}
