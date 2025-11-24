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
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        // Boat data untuk demo
        private List<Boat> boats;

        public DashboardWindow()
        {
            InitializeComponent();
            LoadBoats();
            AttachButtonClickHandlers();
        }

        private void LoadBoats()
        {
            boats = new List<Boat>
            {
                new Boat
                {
                    ID = 1,
                    Name = "Kapal Ferry",
                    Model = "Ferry",
                    Year = 2024,
                    Capacity = 50,
                    Rating = 4.5,
                    PricePerDay = 3500000,
                    Available = true,
                    ThumbnailPath = "Assets/ferry.jpeg"
                },
                new Boat
                {
                    ID = 2,
                    Name = "Speedboat",
                    Model = "Speedboat",
                    Year = 2018,
                    Capacity = 8,
                    Rating = 4.2,
                    PricePerDay = 1200000,
                    Available = false,
                    ThumbnailPath = "Assets/speedboat.jpeg"
                },
                new Boat
                {
                    ID = 3,
                    Name = "Yacht",
                    Model = "Yacht",
                    Year = 2020,
                    Capacity = 12,
                    Rating = 4.8,
                    PricePerDay = 2800000,
                    Available = true,
                    ThumbnailPath = "Assets/yacht.jpeg"
                }
            };
        }

        private void AttachButtonClickHandlers()
        {
            // Find all buttons in BoatPanel and attach click handlers
            foreach (var button in FindAllButtons(BoatPanel))
            {
                if (button.Content.ToString() == "Pilih Kapal Ini")
                {
                    button.Click += SelectBoat_Click;
                }
            }
        }

        private IEnumerable<Button> FindAllButtons(DependencyObject parent)
        {
            var children = LogicalTreeHelper.GetChildren(parent);
            foreach (var child in children)
            {
                if (child is Button btn)
                {
                    yield return btn;
                }
                if (child is DependencyObject dep)
                {
                    foreach (var nestedBtn in FindAllButtons(dep))
                    {
                        yield return nestedBtn;
                    }
                }
            }
        }

        private void SelectBoat_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int boatId = int.Parse(button.Tag.ToString());
                var selectedBoat = boats.FirstOrDefault(b => b.ID == boatId);

                if (selectedBoat != null)
                {
                    // Open rental booking window
                    RentalBookingWindow bookingWindow = new RentalBookingWindow(selectedBoat);
                    bookingWindow.Show();
                }
            }
        }
    }
}
