using System.Windows;
using System.Windows.Controls;
using CobaHW7.ViewModels;
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class DashboardAdmin : Window
    {
        public DashboardAdmin()
        {
            InitializeComponent();
        }

        private DashboardAdminViewModel VM => DataContext as DashboardAdminViewModel;

        private void AddBoat_Click(object sender, RoutedEventArgs e) => VM?.AddBoat();

        private void DeleteBoat_Click(object sender, RoutedEventArgs e)
        {
            if (VM?.BoatsView?.CurrentItem is Boat boat) VM.DeleteBoat(boat);
        }

        private void DeleteBoatRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Boat b) VM?.DeleteBoat(b);
        }

        private void SaveBoats_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Perubahan kapal disimpan (placeholder).", "Admin");
        }

        private void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booking bk) VM?.ConfirmBooking(bk);
        }

        private void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booking bk) VM?.CancelBooking(bk);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
