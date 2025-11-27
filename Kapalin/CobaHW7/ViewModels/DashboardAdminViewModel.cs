using CobaHW7.Class;
using CobaHW7.Services;
using CobaHW7.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CobaHW7.ViewModels
{
    public class DashboardAdminViewModel : BaseViewModel
    {
        private string _boatFilter = "";
        public string BoatFilter
        {
            get => _boatFilter;
            set
            {
                _boatFilter = value;
                OnPropertyChanged(nameof(BoatFilter));
                BoatsView.Refresh();
            }
        }

        private ObservableCollection<Boat> _allBoats;
        public ICollectionView BoatsView { get; private set; }
        public ObservableCollection<Booking> Bookings { get; private set; }


        public ICommand AddBoatCommand { get; }
        public ICommand EditBoatCommand { get; }
        public ICommand DeleteBoatCommand { get; }

        public ICommand ConfirmBookingCommand { get; }
        public ICommand CancelBookingCommand { get; }


        private string? _largeImageUrl;
        public string? LargeImageUrl
        {
            get => _largeImageUrl;
            set
            {
                if (_largeImageUrl != value)
                {
                    _largeImageUrl = value;
                    OnPropertyChanged(nameof(LargeImageUrl));
                }
            }
        }

        public ICommand ShowImageCommand { get; }
        public ICommand HideImageCommand { get; }


        public DashboardAdminViewModel()
        {
            _allBoats = new ObservableCollection<Boat>();
            Bookings = new ObservableCollection<Booking>();

            BoatsView = CollectionViewSource.GetDefaultView(_allBoats);
            BoatsView.SortDescriptions.Add(new SortDescription(nameof(Boat.ID), ListSortDirection.Ascending));
            BoatsView.Filter = FilterBoats;

            AddBoatCommand = new RelayCommand(ExecuteAddBoat);
            EditBoatCommand = new RelayCommand(ExecuteEditBoat);
            DeleteBoatCommand = new RelayCommand(ExecuteDeleteBoat);

            ConfirmBookingCommand = new RelayCommand(ExecuteConfirmBooking);
            CancelBookingCommand = new RelayCommand(ExecuteCancelBooking);

            ShowImageCommand = new RelayCommand(ExecuteShowImage);
            HideImageCommand = new RelayCommand(ExecuteHideImage);

            LoadDataAsync();
        }


        private void ExecuteShowImage(object parameter)
        {
            if (parameter is string url && !string.IsNullOrEmpty(url))
            {
                LargeImageUrl = url;
            }
        }

        private void ExecuteHideImage(object parameter)
        {
            LargeImageUrl = null;
        }

        private async void ExecuteAddBoat(object parameter)
        {
            AddBoatWindow addWindow = new AddBoatWindow();
            addWindow.Owner = Application.Current.MainWindow;

            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                Boat? boatFromForm = addWindow.NewBoat;

                if (boatFromForm == null) return;

                try
                {
                    Boat boatFromDb = await SupabaseService.AddBoatAsync(boatFromForm);

                    if (boatFromDb != null)
                    {
                        _allBoats.Add(boatFromDb);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Gagal menyimpan kapal baru: {ex.Message}");
                    MessageBox.Show("Gagal menyimpan data ke database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ExecuteEditBoat(object parameter)
        {
            if (parameter is Boat boatToEdit)
            {
                AddBoatWindow editWindow = new AddBoatWindow(boatToEdit);
                editWindow.Owner = Application.Current.MainWindow;

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    Boat? updatedBoat = editWindow.NewBoat;
                    if (updatedBoat == null) return;

                    try
                    {
                        Boat resultBoat = await SupabaseService.UpdateBoatAsync(updatedBoat);

                        if (resultBoat != null)
                        {
                            var index = _allBoats.IndexOf(boatToEdit);
                            if (index != -1)
                            {
                                _allBoats[index] = resultBoat;
                            }

                            MessageBox.Show("Data berhasil diperbarui!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Gagal update: {ex.Message}");
                        MessageBox.Show("Gagal update data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void ExecuteDeleteBoat(object parameter)
        {
            if (parameter is Boat boatToDelete)
            {
                var result = MessageBox.Show(
                    $"Apakah Anda yakin ingin menghapus kapal '{boatToDelete.Name}'?\nData yang dihapus tidak dapat dikembalikan.",
                    "Konfirmasi Hapus",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await SupabaseService.DeleteBoatAsync(boatToDelete.ID);

                        _allBoats.Remove(boatToDelete);

                        MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Gagal menghapus data. Pastikan kapal tidak memiliki riwayat booking.\nError: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void ExecuteConfirmBooking(object parameter)
        {
            if (parameter is Booking booking)
            {
                var confirmDialog = new BookingConfirmationWindow(
                    "Konfirmasi Pesanan",
                    $"Apakah Anda yakin ingin mengkonfirmasi pesanan #{booking.BookingId}?",
                    BookingConfirmationWindow.ConfirmationType.Confirm);
                confirmDialog.Owner = Application.Current.MainWindow;
                confirmDialog.ShowDialog();

                if (confirmDialog.IsConfirmed)
                {
                    try
                    {
                        booking.Status = "Confirmed";
                        await SupabaseService.Client
                            .From<Booking>()
                            .Update(booking);

                        LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new BookingConfirmationWindow(
                            "Error",
                            $"Gagal mengkonfirmasi pesanan: {ex.Message}",
                            BookingConfirmationWindow.ConfirmationType.Cancel);
                        errorDialog.Owner = Application.Current.MainWindow;
                        errorDialog.ShowDialog();
                    }
                }
            }
        }

        private async void ExecuteCancelBooking(object parameter)
        {
            if (parameter is Booking booking)
            {
                var cancelDialog = new BookingConfirmationWindow(
                    "Batalkan Pesanan",
                    $"Apakah Anda yakin ingin membatalkan pesanan #{booking.BookingId}?",
                    BookingConfirmationWindow.ConfirmationType.Cancel);
                cancelDialog.Owner = Application.Current.MainWindow;
                cancelDialog.ShowDialog();

                if (cancelDialog.IsConfirmed)
                {
                    try
                    {
                        booking.Status = "Cancelled";
                        await SupabaseService.Client
                            .From<Booking>()
                            .Update(booking);

                        LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        var errorDialog = new BookingConfirmationWindow(
                            "Error",
                            $"Gagal membatalkan pesanan: {ex.Message}",
                            BookingConfirmationWindow.ConfirmationType.Cancel);
                        errorDialog.Owner = Application.Current.MainWindow;
                        errorDialog.ShowDialog();
                    }
                }
            }
        }

        private async void LoadDataAsync()
        {
            try
            {
                var boats = await SupabaseService.GetBoatsAsync();
                _allBoats.Clear();
                foreach (var boat in boats)
                {
                    _allBoats.Add(boat);
                }

                var bookings = await SupabaseService.GetBookingsAsync();
                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    Bookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Gagal memuat data: {ex.Message}");
            }
        }

        private bool FilterBoats(object item)
        {
            if (string.IsNullOrEmpty(BoatFilter)) return true;

            if (item is Boat boat)
            {
                string name = boat.Name ?? "";
                string location = boat.Location ?? "";

                return name.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                       location.IndexOf(BoatFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }
    }
}
