using System;
using System.Collections.Generic;
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

            // Hubungkan dengan ViewModel
            this.DataContext = new DashboardUserViewModel();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Debug: tampilkan MessageBox terlebih dahulu
                MessageBox.Show("Logout button clicked!", "Debug");

                // Disable button to prevent multiple clicks
                var button = sender as Button;
                if (button != null)
                    button.IsEnabled = false;

                // Tampilkan alert info sedang logout
                var logoutAlert = new AlertWindow("Sedang Logout",
                    "Anda sedang logout dari aplikasi. Harap tunggu...",
                    AlertWindow.AlertType.Info);
                logoutAlert.ShowDialog();

                // Sign out dari Supabase (non-async version)
                try
                {
                    // Attempt async sign out
                    var signOutTask = SupabaseService.Client.Auth.SignOut();
                    signOutTask.Wait(5000); // Wait max 5 seconds
                }
                catch
                {
                    // Ignore errors from Supabase, proceed to logout anyway
                }

                // Tampilkan success message
                var successAlert = new AlertWindow("Logout Berhasil!",
                    "Anda telah berhasil logout. Silakan login kembali.",
                    AlertWindow.AlertType.Success);
                successAlert.ShowDialog();

                // Buka MainWindow (Login Page)
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();

                // Tutup Dashboard
                this.Close();
            }
            catch (Exception ex)
            {
                // Jika ada error, tetap buka MainWindow tapi tampilkan warning
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
