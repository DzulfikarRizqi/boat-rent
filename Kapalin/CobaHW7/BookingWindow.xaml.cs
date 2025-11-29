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
using CobaHW7.Class;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class BookingWindow : Window
    {
        public BookingWindow(List<Booking> bookings = null)
        {
            InitializeComponent();
            LoadBookingAsync(bookings);
        }

        private async void LoadBookingAsync(List<Booking> bookings)
        {
            if (bookings == null || bookings.Count == 0)
            {
                // Default demo booking
                var bookingList = new List<dynamic>
                {
                    new { ID = 1, Model = "Kapal Ferry", Price = "Rp 3.500.000", Status = "Dipesan" },
                    new { ID = 3, Model = "Yacht", Price = "Rp 2.800.000", Status = "Dipesan" }
                };
                BookingList.ItemsSource = bookingList;
            }
            else
            {
                // Ambil semua kapal untuk mendapatkan nama berdasarkan ID
                var boatsResponse = await SupabaseService.Client
                    .From<Boat>()
                    .Get();

                var boatDictionary = boatsResponse.Models.ToDictionary(boat => boat.ID, boat => boat.Name);

                // Display actual bookings from database dengan nama kapal
                var displayBookings = bookings.Select(b => new
                {
                    BookingId = b.BookingId,
                    BoatName = boatDictionary.ContainsKey(b.BoatId) ? boatDictionary[b.BoatId] : "Kapal Tidak Ditemukan",
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    TotalAmount = b.TotalAmount,
                    PaymentMethod = b.PaymentMethod,
                    Status = b.Status
                }).ToList();

                BookingList.ItemsSource = displayBookings;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}