using CobaHW7.Class;
using CobaHW7.ViewModels;
using CobaHW7.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CobaHW7
{
    public partial class DashboardAdmin : Window
    {
        public DashboardAdmin()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!BoatsDataGrid.IsMouseOver)
            {
                BoatsDataGrid.UnselectAll();
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
