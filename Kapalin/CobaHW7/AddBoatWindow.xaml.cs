using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CobaHW7.Class; // Pastikan namespace ini benar

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for AddBoatWindow.xaml
    /// </summary>
    
    public partial class AddBoatWindow : Window
    {
        public Boat NewBoat { get; private set; }
        public AddBoatWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validasi sederhana (bisa Anda kembangkan)
            if (string.IsNullOrWhiteSpace(IdTextBox.Text) || string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("ID, Nama, dan Harga tidak boleh kosong.", "Error Validasi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Buat objek Boat baru dari input form
                NewBoat = new Boat
                {
                    ID = long.Parse(IdTextBox.Text),
                    Name = NameTextBox.Text,
                    Model = ModelTextBox.Text,
                    Location = LocationTextBox.Text,
                    Capacity = int.Parse(CapacityTextBox.Text),
                    Year = int.Parse(YearTextBox.Text),
                    Rating = double.Parse(RatingTextBox.Text),
                    PricePerDay = decimal.Parse(PriceTextBox.Text),
                    Available = AvailableCheckBox.IsChecked ?? false,
                    ThumbnailPath = "" // Bisa ditambahkan nanti
                };

                // Set DialogResult ke true agar window utama tahu kita klik "Simpan"
                this.DialogResult = true;
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Pastikan ID dan Angka (Kapasitas, Tahun, Rating, Harga) diisi dengan benar.", "Error Format", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
