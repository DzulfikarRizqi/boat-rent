using CobaHW7.ViewModels;
using System.Windows;

namespace CobaHW7
{
    public partial class ForecastWindow : Window
    {
        private string _location;
        private Window _dashboardWindow;

        public ForecastWindow(string location, Window dashboardWindow = null)
        {
            InitializeComponent();
            _location = location;
            _dashboardWindow = dashboardWindow;

            // Hide Dashboard window jika ada
            if (_dashboardWindow != null)
            {
                _dashboardWindow.Hide();
            }

            // Load forecast data
            if (this.DataContext is ForecastViewModel viewModel)
            {
                viewModel.LoadForecast(location);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Show Dashboard window kembali
            if (_dashboardWindow != null)
            {
                _dashboardWindow.Show();
            }

            // Close forecast window
            this.Close();
        }
    }
}
