using System;
using System.Windows;
using CobaHW7.Class;
using CobaHW7.Services;

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

            BoatNameText.Text = selectedBoat.Name;
            BoatYearText.Text = selectedBoat.Year.ToString();
            BoatLocationText.Text = selectedBoat.Location;
            BoatCapacityText.Text = $"{selectedBoat.Capacity} orang";
            pricePerDay = selectedBoat.PricePerDay;
            BoatPriceText.Text = $"Rp {pricePerDay:N0} / hari";

            if (!string.IsNullOrEmpty(selectedBoat.ThumbnailPath))
            {
                try
                {
                    if (selectedBoat.ThumbnailPath.StartsWith("http://") || selectedBoat.ThumbnailPath.StartsWith("https://"))
                    {
                        BoatImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(selectedBoat.ThumbnailPath));
                    }
                    else
                    {
                        BoatImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedBoat.ThumbnailPath)));
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
            UpdateTotal();
        }

        private void SetDatePickerConstraints()
        {
            StartDatePicker.DisplayDateStart = DateTime.Today;
            StartDatePicker.SelectedDate = DateTime.Today;

            EndDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void StartDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate.HasValue)
            {
                //set end date = start date + 1
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                EndDatePicker.DisplayDateStart = startDate.AddDays(1);

                //adjust end date kalo > start date + 1
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

            int days = (int)(endDate - startDate).TotalDays;
            if (days < 1)
            {
                days = 1;
            }
            decimal total = pricePerDay * days;
            TotalText.Text = $"Rp {total:N0}";
            DaysText.Text = $"({days} {(days == 1 ? "hari" : "hari")})";
        }

        private void PaymentMethod_Changed(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BookNowButton_Click(object sender, RoutedEventArgs e)
        {
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

            string paymentMethod = QrisRadio.IsChecked == true ? "QRIS" : "Transfer Virtual Account";
            int days = (int)(endDate - startDate).TotalDays;
            decimal total = pricePerDay * days;

            try
            {
                // Check ketersediaan kapal - apakah sudah ada 2 booking dengan overlap tanggal
                var (available, message) = await SupabaseService.CheckBoatAvailabilityAsync(selectedBoat.ID.Value, startDate, endDate);

                if (!available)
                {
                    var alert = new AlertWindow("Kapal Tidak Tersedia", message, AlertWindow.AlertType.Warning);
                    alert.ShowDialog();
                    return;
                }

                // Store booking data untuk digunakan di payment window
                var bookingData = new
                {
                    BoatId = selectedBoat.ID,
                    BoatName = selectedBoat.Name,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalAmount = total,
                    PaymentMethod = paymentMethod
                };
                //buka payment window berdasarkan metode pembayaran yg dipilih
                bool paymentConfirmed = false;

                if (paymentMethod == "QRIS")
                {
                    QrisPaymentWindow qrisWindow = new QrisPaymentWindow(total, bookingData);
                    qrisWindow.ShowDialog();
                    paymentConfirmed = qrisWindow.IsConfirmed;
                }
                else
                {
                    VirtualAccountPaymentWindow vaWindow = new VirtualAccountPaymentWindow(total, bookingData);
                    vaWindow.ShowDialog();
                    paymentConfirmed = vaWindow.IsConfirmed;
                }

                if (paymentConfirmed)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                var alert = new AlertWindow("Pemesanan Gagal", $"Terjadi kesalahan: {ex.Message}", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
        }
    }
}
