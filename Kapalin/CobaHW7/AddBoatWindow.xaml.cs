using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;       // <-- TAMBAHKAN INI
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32; // <-- TAMBAHKAN INI
using CobaHW7.Class; // Pastikan namespace ini benar
using CobaHW7.Services; // <-- TAMBAHKAN BARIS INI

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for AddBoatWindow.xaml
    /// </summary>
    
    public partial class AddBoatWindow : Window
    {
        public Boat? NewBoat { get; private set; }

        private string _selectedThumbnailPath = ""; // <-- BARU
        public AddBoatWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                Title = "Pilih Gambar Kapal"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Simpan path file
                _selectedThumbnailPath = openFileDialog.FileName;
                // Tampilkan di TextBox
                ThumbnailTextBox.Text = _selectedThumbnailPath;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
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
                string thumbnailUrl = ""; // Default URL kosong

                // 1. Cek jika user memilih file gambar
                if (!string.IsNullOrEmpty(_selectedThumbnailPath))
                {
                    // Buat nama file unik, misal: "boat_ID_namafile.jpg"
                    string extension = System.IO.Path.GetExtension(_selectedThumbnailPath);
                    string fileNameInStorage = $"boat_{IdTextBox.Text}{extension}";

                    // 2. Upload gambar ke Supabase Storage
                    // Ini akan memanggil SupabaseService
                    thumbnailUrl = await SupabaseService.UploadBoatImageAsync(_selectedThumbnailPath, fileNameInStorage);
                }

                // 3. Buat objek Boat baru, SEKARANG TERMASUK URL GAMBAR
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
                    ThumbnailPath = thumbnailUrl // <-- SIMPAN URL, BUKAN PATH LOKAL
                };

                // 4. Tutup window (ViewModel akan mengambil NewBoat)
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
