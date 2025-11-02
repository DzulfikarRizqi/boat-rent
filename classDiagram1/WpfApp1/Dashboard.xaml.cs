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
using WpfApp1.Class;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBoats();
        }

        private void LoadBoats()
        {
            // Data kapal
            List<Boat> boats = new List<Boat>
            {
                new Boat(1, "Kapal Ferry", 5000000, 2020, "Assets/ferry.jpg", true),
                new Boat(2, "Speed Boat", 7500000, 2022, "Assets/speedboat.jpg", false),
                new Boat(3, "Yacht", 12000000, 2021, "Assets/yacht.jpg", true)
            };

            BoatPanel.Children.Clear();

            foreach (var boat in boats)
            {
                var card = new Border
                {
                    Width = 250,
                    Height = 340,
                    Margin = new Thickness(10),
                    CornerRadius = new CornerRadius(10),
                    BorderThickness = new Thickness(1),
                    BorderBrush = System.Windows.Media.Brushes.LightGray,
                    Background = System.Windows.Media.Brushes.White
                };

                var stack = new StackPanel { Margin = new Thickness(10) };

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(boat.Image, UriKind.Relative)),
                    Height = 150,
                    Stretch = System.Windows.Media.Stretch.UniformToFill,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var title = new TextBlock
                {
                    Text = boat.Model,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };

                var info = new TextBlock
                {
                    Text = $"Tahun: {boat.Year}\nHarga: Rp {boat.Price:N0} / hari\nStatus: {(boat.Available ? "Tersedia" : "Tidak Tersedia")}",
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var button = new Button
                {
                    Content = "Pilih Kapal Ini",
                    Background = boat.Available
                        ? System.Windows.Media.Brushes.SteelBlue
                        : System.Windows.Media.Brushes.Gray,
                    Foreground = System.Windows.Media.Brushes.White,
                    IsEnabled = boat.Available,
                    Padding = new Thickness(5),
                    Tag = boat.ID,
                    Width = 150,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                button.Click += SelectBoat_Click;

                stack.Children.Add(image);
                stack.Children.Add(title);
                stack.Children.Add(info);
                stack.Children.Add(button);
                card.Child = stack;
                BoatPanel.Children.Add(card);
            }
        }

        private void SelectBoat_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            MessageBox.Show("Kapal berhasil dipilih!", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CekBooking_Click(object sender, RoutedEventArgs e)
        {
            BookingWindow bookingWindow = new BookingWindow();
            bookingWindow.Show();
        }


        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Anda berhasil logout.", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
