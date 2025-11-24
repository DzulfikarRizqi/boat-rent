using System;
using System.Windows;

namespace CobaHW7
{
    public partial class QrisPaymentWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public QrisPaymentWindow(decimal amount)
        {
            InitializeComponent();
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

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }
    }
}
