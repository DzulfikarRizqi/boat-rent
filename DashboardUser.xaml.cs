using CobaHW7.Class;
using CobaHW7.ViewModels;
using System.Windows;
using System.Windows.Controls;
using CobaHW7;

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
            if (sender is Button btn && btn.Tag is Boat boat)
            {
                var win = new BookingWindow
                {
                    Owner = this
                };
                win.ShowDialog();
            }
        }

        private void BtnCekBooking_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("(Placeholder) Di sini tampilkan daftar booking user.", "Cek Booking");
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logout sukses.", "Kapalku");
            Close();
        }
    }
}
