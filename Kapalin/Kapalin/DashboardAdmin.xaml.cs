using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CobaHW7.ViewModels;
using CobaHW7.Class;
using CobaHW7.Data;
using System.Linq;
using CobaHW7.Data;

namespace CobaHW7
{
    public partial class DashboardAdmin : Window
    {
        public DashboardAdmin()
        {
            InitializeComponent();
            DataContext = new DashboardAdminViewModel();
            Loaded += DashboardAdmin_Loaded;
        }

        private DashboardAdminViewModel? VM => DataContext as DashboardAdminViewModel;

        private async void DashboardAdmin_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFromDb();
        }

        private async Task LoadFromDb()
        {
            if (VM == null) return;

            // load boats
            var boatRepo = new CobaHW7.Data.BoatRepository();
            var boats = await boatRepo.GetAllAsync();
            VM.Boats.Clear();
            foreach (var b in boats)
                VM.Boats.Add(new Boat
                {
                    ID = b.ID,
                    Name = b.Name,
                    Model = b.Model,
                    PricePerDay = (decimal)b.Price,
                    Available = b.Available,
                    Location = ""   // isi kalau tabel ada
                });

            // load bookings
            var bookingRepo = new BookingRepository();
            var bookings = await bookingRepo.GetAllAsync();
            VM.Bookings.Clear();
            foreach (var bk in bookings)
                VM.Bookings.Add(bk);
        }

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
            MessageBox.Show("Belum di-save ke DB. Tambah repo update kalau mau persist.");
        }

        private async void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booking bk)
            {
                VM?.ConfirmBooking(bk);
                // TODO: update status ke DB kalau mau
                await Task.CompletedTask;
            }
        }

        private async void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Booking bk)
            {
                VM?.CancelBooking(bk);
                // TODO: update DB
                await Task.CompletedTask;
            }
        }
    }
}
