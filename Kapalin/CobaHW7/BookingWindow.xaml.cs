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

namespace CobaHW7
{
    /// <summary>
    /// Interaction logic for BookingWindow.xaml
    /// </summary>
    public partial class BookingWindow : Window
    {
        public BookingWindow()
        {
            InitializeComponent();
            LoadBooking();
        }
        private void LoadBooking()
        {
            var bookingList = new List<dynamic>
            {
                new { ID = 1, Model = "Kapal Ferry", Price = "Rp 3.500.000", Status = "Dipesan" },
                new { ID = 3, Model = "Yacht", Price = "Rp 2.800.000", Status = "Dipesan" }
            };

            BookingList.ItemsSource = bookingList;
        }
    }
}