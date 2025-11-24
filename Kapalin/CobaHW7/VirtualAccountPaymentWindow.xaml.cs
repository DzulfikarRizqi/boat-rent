using System;
using System.Windows;

namespace CobaHW7
{
    public partial class VirtualAccountPaymentWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public VirtualAccountPaymentWindow(decimal amount)
        {
            InitializeComponent();
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

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }
    }
}
