using System;
using System.Windows;
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class VirtualAccountPaymentWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;
        private dynamic bookingData;

        public VirtualAccountPaymentWindow(decimal amount, dynamic bookingData = null)
        {
            InitializeComponent();
            this.bookingData = bookingData;
            LoadPaymentDetails(amount);
        }

        private void LoadPaymentDetails(decimal amount)
        {
            // Set default bank selection
            BankComboBox.SelectedIndex = 0;

            // Display nominal
            NominalText.Text = $"Rp {amount:N0}";

            // Generate Virtual Account Number
            // Format: BANK CODE + USER ID + BOOKING ID (simulated)
            GenerateVANumber();
        }

        private void GenerateVANumber()
        {
            string bankCode = GetBankCode(BankComboBox.SelectedIndex);
            string vaNumber = $"{bankCode}{DateTime.Now:yyyyMMdd}{new Random().Next(100000, 999999)}";
            VANumberText.Text = FormatVANumber(vaNumber);
        }

        private string GetBankCode(int selectedIndex)
        {
            return selectedIndex switch
            {
                0 => "014", // BCA
                1 => "008", // Mandiri
                2 => "009", // BNI
                3 => "002", // BRI
                _ => "014"
            };
        }

        private string FormatVANumber(string number)
        {
            // Format: XXX XXXX XXXX XXXX
            if (number.Length >= 16)
            {
                return $"{number.Substring(0, 3)} {number.Substring(3, 4)} {number.Substring(7, 4)} {number.Substring(11)}";
            }
            return number;
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vaNumber = VANumberText.Text.Replace(" ", "");
                System.Windows.IDataObject dataObject = new System.Windows.DataObject();
                dataObject.SetData(System.Windows.DataFormats.Text, vaNumber);
                System.Windows.Clipboard.SetDataObject(dataObject);
                MessageBox.Show("Nomor Virtual Account telah disalin ke clipboard.", "Sukses");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyalin: {ex.Message}", "Error");
            }
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
                    PaymentMethod = "Transfer Virtual Account",
                    Status = "Menunggu Pembayaran"
                };

                // Save to Supabase
                try
                {
                    var result = await SupabaseService.Client.From<Booking>().Insert(booking);

                    // Show success message
                    var successAlert = new AlertWindow("Pemesanan Berhasil!",
                        $"Pesanan Anda untuk {bookingData.BoatName} telah dibuat.\n\nTotal: Rp {bookingData.TotalAmount:N0}\nMetode Pembayaran: Transfer Virtual Account",
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
