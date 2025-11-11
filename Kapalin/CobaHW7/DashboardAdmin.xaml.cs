using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CobaHW7.ViewModels;
using CobaHW7.Class;   // Boat, Booking, BookingStatus
using CobaHW7.Data;    // BoatRepository, BookingRepository

namespace CobaHW7
{
    public partial class DashboardAdmin : Window
    {
        private readonly BookingRepository _bookingRepo = new();
        private readonly BoatRepository _boatRepo = new();

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

            // ---------- Load Boats ----------
            var boats = await _boatRepo.GetAllAsync();
            VM.Boats.Clear();
            foreach (var b in boats)
            {
                // mapping ke model Boat milik ViewModel
                VM.Boats.Add(new Boat
                {
                    ID = b.ID,
                    Name = b.Name,
                    Model = b.Model,
                    PricePerDay = (decimal)b.Price,
                    Available = b.Available,
                    Location = "" // isi jika kamu punya kolom lokasi
                });
            }

            // ---------- Load Bookings ----------
            var rows = await _bookingRepo.GetAllAsync(); // List<BookingRow>
            VM.Bookings.Clear();
            foreach (var r in rows)
                VM.Bookings.Add(MapToBooking(r));
        }

        // ===================== Helpers =====================
        private static Booking MapToBooking(BookingRow r)
        {
            return new Booking
            {
                BookingId = r.Id,
                UserId = r.UserId,
                BoatId = r.BoatId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalAmount = r.TotalAmount,
                Status = ParseStatus(r.Status)
            };
        }

        // DB menyimpan "Pending/Confirmed/Cancelled" (atau kadang Indonesia),
        // terima keduanya dan kembalikan enum BookingStatus kamu.
        private static BookingStatus ParseStatus(string? s)
        {
            switch ((s ?? "").Trim().ToLowerInvariant())
            {
                case "pending": return BookingStatus.Pending;

                // confirmed
                case "confirmed":
                case "dipesan":
                case "selesai":
                case "completed": return BookingStatus.Confirmed;

                // cancelled
                case "cancelled":
                case "dibatalkan": return BookingStatus.Cancelled;

                default: return BookingStatus.Pending;
            }
        }

        // ===================== UI Actions =====================
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
            MessageBox.Show("Belum di-save ke DB. Tambahkan method repo untuk persist perubahan boat jika diperlukan.");
        }

        private async void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (VM == null) return;

            var booking = ExtractBookingFromSender(sender);
            if (booking == null) return;

            booking.Status = BookingStatus.Confirmed;
            VM.ConfirmBooking(booking);

            // Persist ke DB (opsional tapi direkomendasikan)
            try { await _bookingRepo.UpdateStatusAsync(booking.BookingId, "Confirmed"); }
            catch (Exception ex) { DialogService.Error("Gagal mengonfirmasi booking", ex.Message); }
        }

        private async void CancelBooking_Click(object sender, RoutedEventArgs e)
        {
            if (VM == null) return;

            var booking = ExtractBookingFromSender(sender);
            if (booking == null) return;

            booking.Status = BookingStatus.Cancelled;
            VM.CancelBooking(booking);

            try { await _bookingRepo.UpdateStatusAsync(booking.BookingId, "Cancelled"); }
            catch (Exception ex) { DialogService.Error("Gagal membatalkan booking", ex.Message); }
        }

        // Ambil booking dari Button.Tag (bisa BookingRow atau Booking)
        private Booking? ExtractBookingFromSender(object sender)
        {
            if (sender is not Button btn) return null;

            return btn.Tag switch
            {
                Booking b => b,
                BookingRow br => MapToBooking(br),
                _ => null
            };
        }
    }
}
