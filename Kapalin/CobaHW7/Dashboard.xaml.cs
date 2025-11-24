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
            this.DataContext = new DashboardUserViewModel();
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
