using System;
using System.Collections.Generic;
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
            NominalText.Text = $"Rp {amount:N0}";

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
                var confirmBtn = sender as Button;
                if (confirmBtn != null)
                    confirmBtn.IsEnabled = false;

                var processingAlert = new AlertWindow("Pesanan Sedang Diproses",
                    "Pesanan Anda sedang kami proses. Harap tunggu sebentar...",
                    AlertWindow.AlertType.Info);
                processingAlert.ShowDialog();

                var currentUser = SupabaseService.Client.Auth.CurrentUser;
                if (currentUser == null)
                {
                    var errorAlert = new AlertWindow("Error",
                        "User tidak ditemukan. Silakan login kembali.",
                        AlertWindow.AlertType.Error);
                    errorAlert.ShowDialog();
                    if (confirmBtn != null)
                        confirmBtn.IsEnabled = true;
                    return;
                }

                var booking = new Booking
                {
                    UserId = currentUser.Id,
                    BoatId = bookingData.BoatId,
                    StartDate = bookingData.StartDate,
                    EndDate = bookingData.EndDate,
                    TotalAmount = bookingData.TotalAmount,
                    PaymentMethod = "QRIS",
                    Status = "Menunggu Konfirmasi"
                };

                try
                {
                    //Supabase RPC function insert booking (repot brbrbr)
                    var result = await SupabaseService.Client.Rpc("insert_booking", new Dictionary<string, object>
                    {
                        { "p_user_id", booking.UserId },
                        { "p_boat_id", booking.BoatId },
                        { "p_start_date", booking.StartDate },
                        { "p_end_date", booking.EndDate },
                        { "p_total_amount", booking.TotalAmount },
                        { "p_payment_method", booking.PaymentMethod },
                        { "p_status", booking.Status }
                    });

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
