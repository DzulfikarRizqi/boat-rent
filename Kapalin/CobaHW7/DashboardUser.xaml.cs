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
            MessageBox.Show("Logout sukses.", "Kapalku");
            Close();
        }
    }
}
