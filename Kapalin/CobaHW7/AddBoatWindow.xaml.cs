using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for AddBoatWindow.xaml
    /// </summary>
    public partial class AddBoatWindow : Window
    {
        public Boat? NewBoat { get; private set; }

        // Menyimpan path file lokal jika user memilih gambar baru
        private string _selectedThumbnailPath = "";

        // Menyimpan URL gambar yang SUDAH ADA (untuk mode Edit)
        private string _existingImageUrl = "";

        // --- CONSTRUCTOR 1: Mode Tambah Baru (Default) ---
        public AddBoatWindow()
        {
            InitializeComponent();
            Title = "Tambah Kapal Baru"; // Judul Window
        }

        // --- CONSTRUCTOR 2: Mode Edit (Baru) ---
        // Dipanggil saat tombol "Edit" diklik di Dashboard
        public AddBoatWindow(Boat boatToEdit) : this() // Panggil constructor dasar dulu
        {
            Title = "Edit Data Kapal"; // Ubah Judul Window

            // 1. Isi form dengan data yang sudah ada
            IdTextBox.Text = boatToEdit.ID.ToString();

            // PENTING: Kunci ID agar tidak bisa diubah (Primary Key tidak boleh ganti)
            IdTextBox.IsReadOnly = true;
            IdTextBox.Background = Brushes.LightGray;

            NameTextBox.Text = boatToEdit.Name;
            ModelTextBox.Text = boatToEdit.Model;
            LocationTextBox.Text = boatToEdit.Location;
            CapacityTextBox.Text = boatToEdit.Capacity.ToString();
            YearTextBox.Text = boatToEdit.Year.ToString();
            RatingTextBox.Text = boatToEdit.Rating.ToString();
            PriceTextBox.Text = boatToEdit.PricePerDay.ToString();
            AvailableCheckBox.IsChecked = boatToEdit.Available;

            // 2. Simpan URL gambar lama
            if (!string.IsNullOrEmpty(boatToEdit.ThumbnailPath))
            {
                _existingImageUrl = boatToEdit.ThumbnailPath;
                ThumbnailTextBox.Text = _existingImageUrl; // Tampilkan URL di kotak teks
            }
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
                // Simpan path file baru yang dipilih user
                _selectedThumbnailPath = openFileDialog.FileName;
                // Tampilkan path di TextBox
                ThumbnailTextBox.Text = _selectedThumbnailPath;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validasi sederhana
            if (string.IsNullOrWhiteSpace(IdTextBox.Text) || string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("ID, Nama, dan Harga tidak boleh kosong.", "Error Validasi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Logika Gambar:
                // Mulai dengan asumsi kita pakai gambar lama (jika sedang edit)
                string finalThumbnailUrl = _existingImageUrl;

                // Jika user memilih file gambar BARU lewat tombol Browse
                if (!string.IsNullOrEmpty(_selectedThumbnailPath))
                {
                    // Buat nama file unik
                    string extension = System.IO.Path.GetExtension(_selectedThumbnailPath);
                    string fileNameInStorage = $"boat_{IdTextBox.Text}{extension}";

                    // Upload gambar BARU ke Supabase Storage dan dapatkan URL baru
                    finalThumbnailUrl = await SupabaseService.UploadBoatImageAsync(_selectedThumbnailPath, fileNameInStorage);
                }

                // Buat objek Boat baru (atau update yang lama)
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

                    // Gunakan URL yang sudah ditentukan (bisa baru, bisa lama)
                    ThumbnailPath = finalThumbnailUrl
                };

                // Tutup window dan beri tahu sukses
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