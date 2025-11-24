using System;
using System.Windows;
using System.Windows.Controls;
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class QrisPaymentWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;
        private dynamic bookingData;

        public QrisPaymentWindow(decimal amount, dynamic bookingData = null)
        {
            InitializeComponent();
            this.bookingData = bookingData;
            LoadPaymentDetails(amount);
        }

        private void LoadPaymentDetails(decimal amount)
        {
            // Display nominal
            NominalText.Text = $"Rp {amount:N0}";

            // Generate reference number (format: QRIS-YYYYMMDD-HHMMSS-RANDOM)
            string referenceNumber = $"QRIS-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            ReferenceText.Text = referenceNumber;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable button saat processing
                var confirmBtn = sender as Button;
                if (confirmBtn != null)
                    confirmBtn.IsEnabled = false;

                // Show processing alert
                var processingAlert = new AlertWindow("Pesanan Sedang Diproses",
                    "Pesanan Anda sedang kami proses. Harap tunggu sebentar...",
                    AlertWindow.AlertType.Info);
                processingAlert.ShowDialog();

                // Create booking object
                var booking = new Booking
                {
                    BoatId = bookingData.BoatId,
                    StartDate = bookingData.StartDate,
                    EndDate = bookingData.EndDate,
                    TotalAmount = bookingData.TotalAmount,
                    PaymentMethod = "QRIS",
                    Status = "Menunggu Pembayaran"
                };

                // Save to Supabase
                try
                {
                    var result = await SupabaseService.Client.From<Booking>().Insert(booking);

                    // Show success message
                    var successAlert = new AlertWindow("Pemesanan Berhasil!",
                        $"Pesanan Anda untuk {bookingData.BoatName} telah dibuat.\n\nTotal: Rp {bookingData.TotalAmount:N0}\nMetode Pembayaran: QRIS",
                        AlertWindow.AlertType.Success);
                    successAlert.ShowDialog();

                    IsConfirmed = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    var errorAlert = new AlertWindow("Gagal Menyimpan Booking",
                        $"Terjadi kesalahan saat menyimpan pesanan: {ex.Message}",
                        AlertWindow.AlertType.Error);
                    errorAlert.ShowDialog();

                    if (confirmBtn != null)
                        confirmBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                var alert = new AlertWindow("Error", $"Terjadi kesalahan: {ex.Message}", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
        }
    }
}
