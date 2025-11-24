using System;
using System.Windows;
using CobaHW7.Class;

namespace CobaHW7
{
    public partial class RentalBookingWindow : Window
    {
        private Boat selectedBoat;
        private decimal pricePerDay;

        public RentalBookingWindow(Boat boat)
        {
            InitializeComponent();
            selectedBoat = boat;
            LoadBoatDetails();
            SetDatePickerConstraints();
        }

        private void LoadBoatDetails()
        {
            if (selectedBoat == null) return;

            // Load boat information
            BoatNameText.Text = selectedBoat.Name;
            BoatYearText.Text = selectedBoat.Year.ToString();
            BoatCapacityText.Text = $"{selectedBoat.Capacity} orang";
            pricePerDay = selectedBoat.PricePerDay;

            // Format price
            BoatPriceText.Text = $"Rp {pricePerDay:N0} / hari";

            // Set image path
            if (!string.IsNullOrEmpty(selectedBoat.ThumbnailPath))
            {
                try
                {
                    // Cek apakah path adalah URL atau local path
                    if (selectedBoat.ThumbnailPath.StartsWith("http://") || selectedBoat.ThumbnailPath.StartsWith("https://"))
                    {
                        // Dari Supabase atau URL lain
                        BoatImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(selectedBoat.ThumbnailPath));
                    }
                    else
                    {
                        // Local path
                        BoatImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedBoat.ThumbnailPath)));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                    // Jika gagal, gunakan placeholder atau biarkan kosong
                }
            }

            // Initialize total display
            UpdateTotal();
        }

        private void SetDatePickerConstraints()
        {
            // Set minimum date to today
            StartDatePicker.DisplayDateStart = DateTime.Today;
            StartDatePicker.SelectedDate = DateTime.Today;

            EndDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void StartDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue)
            {
                // Set minimum end date to start date + 1 day
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                EndDatePicker.DisplayDateStart = startDate.AddDays(1);

                // If end date is before start date, adjust it
                if (EndDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate < startDate.AddDays(1))
                {
                    EndDatePicker.SelectedDate = startDate.AddDays(1);
                }
            }

            UpdateTotal();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                TotalText.Text = "Rp 0";
                DaysText.Text = "0 hari";
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            // Calculate number of days
            int days = (int)(endDate - startDate).TotalDays;

            if (days < 1)
            {
                days = 1;
            }

            // Calculate total
            decimal total = pricePerDay * days;

            // Update display
            TotalText.Text = $"Rp {total:N0}";
            DaysText.Text = $"({days} {(days == 1 ? "hari" : "hari")})";
        }

        private void PaymentMethod_Changed(object sender, RoutedEventArgs e)
        {
            // Method untuk handle perubahan metode pembayaran
            // Bisa digunakan untuk update UI atau validasi tertentu
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BookNowButton_Click(object sender, RoutedEventArgs e)
        {
            // Validasi input
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                var alert = new AlertWindow("Validasi Gagal", "Silakan pilih tanggal mulai dan tanggal selesai.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            if (endDate <= startDate)
            {
                var alert = new AlertWindow("Validasi Gagal", "Tanggal selesai harus setelah tanggal mulai.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            // Get payment method
            string paymentMethod = QrisRadio.IsChecked == true ? "QRIS" : "Transfer Virtual Account";

            // Calculate total
            int days = (int)(endDate - startDate).TotalDays;
            decimal total = pricePerDay * days;

            try
            {
                // Create booking object
                var booking = new Booking
                {
                    BoatId = selectedBoat.ID,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalAmount = total,
                    PaymentMethod = paymentMethod,
                    Status = "Menunggu Pembayaran"
                };

                // TODO: Save to Supabase
                // var result = await SupabaseService.Client.From<Booking>().Insert(booking);

                // Show success message
                var alert = new AlertWindow("Pemesanan Berhasil!",
                    $"Pesanan Anda untuk {selectedBoat.Name} telah dibuat.\n\nTotal: Rp {total:N0}\nMetode Pembayaran: {paymentMethod}",
                    AlertWindow.AlertType.Success);
                alert.ShowDialog();

                // Close this window and go back to dashboard
                this.Close();
            }
            catch (Exception ex)
            {
                var alert = new AlertWindow("Pemesanan Gagal", $"Terjadi kesalahan: {ex.Message}", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
        }
    }
}
