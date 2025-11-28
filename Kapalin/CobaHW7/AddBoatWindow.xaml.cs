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
    public partial class AddBoatWindow : Window
    {
        public Boat? NewBoat { get; private set; }
        private string _selectedThumbnailPath = "";
        private string _existingImageUrl = "";
        private long? _boatIdForEdit = null;

        public AddBoatWindow()
        {
            InitializeComponent();
            Title = "Kelola Data Kapal";
            HeaderTitle.Text = "Tambah Kapal Baru";
            HeaderSubtitle.Text = "Isi data kapal yang akan ditambahkan ke sistem";
        }

        public AddBoatWindow(Boat boatToEdit) : this()
        {
            Title = "Kelola Data Kapal";
            HeaderTitle.Text = "Edit Data Kapal";
            HeaderSubtitle.Text = "Perbarui informasi kapal yang dipilih";

            _boatIdForEdit = boatToEdit.ID;
            NameTextBox.Text = boatToEdit.Name;
            ModelTextBox.Text = boatToEdit.Model;
            LocationTextBox.Text = boatToEdit.Location;
            CapacityTextBox.Text = boatToEdit.Capacity.ToString();
            YearTextBox.Text = boatToEdit.Year.ToString();
            RatingTextBox.Text = boatToEdit.Rating.ToString();
            PriceTextBox.Text = boatToEdit.PricePerDay.ToString();
            AvailableCheckBox.IsChecked = boatToEdit.Available;

            if (!string.IsNullOrEmpty(boatToEdit.ThumbnailPath))
            {
                _existingImageUrl = boatToEdit.ThumbnailPath;
                ThumbnailTextBox.Text = _existingImageUrl;
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
                _selectedThumbnailPath = openFileDialog.FileName;
                ThumbnailTextBox.Text = _selectedThumbnailPath;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                MessageBox.Show("Nama dan Harga tidak boleh kosong.", "Error Validasi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string finalThumbnailUrl = _existingImageUrl;

                if (!string.IsNullOrEmpty(_selectedThumbnailPath))
                {
                    string extension = System.IO.Path.GetExtension(_selectedThumbnailPath);
                    string fileNameInStorage = _boatIdForEdit != null
                        ? $"boat_{_boatIdForEdit}{extension}"
                        : $"boat_{DateTime.Now:yyyyMMddHHmmss}{extension}";

                    finalThumbnailUrl = await SupabaseService.UploadBoatImageAsync(_selectedThumbnailPath, fileNameInStorage);
                }

                NewBoat = new Boat
                {
                    ID = _boatIdForEdit, 
                    Name = NameTextBox.Text,
                    Model = ModelTextBox.Text,
                    Location = LocationTextBox.Text,
                    Capacity = int.Parse(CapacityTextBox.Text),
                    Year = int.Parse(YearTextBox.Text),
                    Rating = double.Parse(RatingTextBox.Text),
                    PricePerDay = decimal.Parse(PriceTextBox.Text),
                    Available = AvailableCheckBox.IsChecked ?? false,
                    ThumbnailPath = finalThumbnailUrl
                };

                this.DialogResult = true;
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Pastikan Angka (Kapasitas, Tahun, Rating, Harga) diisi dengan benar.", "Error Format", MessageBoxButton.OK, MessageBoxImage.Error);
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