using CobaHW7.Class;
using CobaHW7.ViewModels;
using CobaHW7.Services;
using System.Windows;
using System.Windows.Controls;
using CobaHW7;
using System;

namespace CobaHW7
{
    public partial class DashboardUser : Window
    {
        public DashboardUser()
        {
            InitializeComponent();
        }

        private DashboardUserViewModel VM => DataContext as DashboardUserViewModel;

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            VM?.ClearFilters();
        }

        private void SelectBoat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    // Debugging: cek apa yang ada di Tag
                    if (btn.Tag is Boat boat)
                    {
                        var rentalWindow = new RentalBookingWindow(boat);
                        rentalWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show($"Tag is not a Boat. Tag type: {btn.Tag?.GetType().Name ?? "null"}", "Debug");
                    }
                }
                else
                {
                    MessageBox.Show("Sender is not a Button", "Debug");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error in SelectBoat_Click");
            }
        }

        private void BtnCekBooking_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("(Placeholder) Di sini tampilkan daftar booking user.", "Cek Booking");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable button to prevent multiple clicks
                var button = sender as Button;
                if (button != null)
                    button.IsEnabled = false;

                var logoutAlert = new AlertWindow("Sedang Logout",
                    "Anda sedang logout dari aplikasi. Harap tunggu...",
                    AlertWindow.AlertType.Info);
                logoutAlert.ShowDialog();

                try
                {
                    // Attempt async sign out
                    var signOutTask = SupabaseService.Client.Auth.SignOut();
                    signOutTask.Wait(5000); 
                }
                catch
                {

                }

                // Tampilkan success message
                var successAlert = new AlertWindow("Logout Berhasil!",
                    "Anda telah berhasil logout. Silakan login kembali.",
                    AlertWindow.AlertType.Success);
                successAlert.ShowDialog();

                // Buka MainWindow (Login Page)
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();

                // Tutup DashboardUser
                this.Close();
            }
            catch (Exception ex)
            {
                var errorAlert = new AlertWindow("Logout",
                    "Anda akan diarahkan ke halaman login.",
                    AlertWindow.AlertType.Info);
                errorAlert.ShowDialog();

                // Tetap buka MainWindow walaupun ada error
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
