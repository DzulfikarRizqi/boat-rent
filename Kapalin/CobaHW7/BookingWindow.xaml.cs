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

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        public BookingWindow(List<Booking> bookings = null)
        {
            InitializeComponent();
            LoadBooking(bookings);
        }

        private void LoadBooking(List<Booking> bookings)
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
                // Display actual bookings from database
                var displayBookings = bookings.Select(b => new
                {
                    BookingId = b.BookingId,
                    BoatId = b.BoatId,
                    StartDate = b.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = b.EndDate.ToString("yyyy-MM-dd"),
                    TotalAmount = $"Rp {b.TotalAmount:N0}",
                    PaymentMethod = b.PaymentMethod,
                    Status = b.Status
                }).ToList();

                BookingList.ItemsSource = displayBookings;
            }
        }
    }
}