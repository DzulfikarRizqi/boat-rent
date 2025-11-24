using System.Windows;
using System.Windows.Media;

namespace CobaHW7
{
    public partial class AlertWindow : Window
    {
        public AlertWindowResult Result { get; private set; } = AlertWindowResult.None;

        public enum AlertType
        {
            Success,
            Error,
            Warning,
            Info
        }

        public AlertWindow(string title, string message, AlertType type = AlertType.Info, bool showCancelButton = false)
        {
            InitializeComponent();
            SetupAlert(title, message, type, showCancelButton);
        }

        private void SetupAlert(string title, string message, AlertType type, bool showCancelButton)
        {
            TitleText.Text = title;
            MessageText.Text = message;

            // Tampilkan/sembunyikan tombol Cancel
            if (showCancelButton)
            {
                Button2.Visibility = Visibility.Visible;
                Button1.Content = "Ya";
                Button2.Content = "Tidak";
            }

            // Set icon dan warna berdasarkan tipe
            switch (type)
            {
                case AlertType.Success:
                    IconText.Text = "✓";
                    IconText.Foreground = new SolidColorBrush(Color.FromArgb(255, 76, 175, 80)); // Green
                    IconBackground.Fill = new SolidColorBrush(Color.FromArgb(30, 76, 175, 80));
                    Button1.Background = new SolidColorBrush(Color.FromArgb(255, 76, 175, 80));
                    break;

                case AlertType.Error:
                    IconText.Text = "✕";
                    IconText.Foreground = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54)); // Red
                    IconBackground.Fill = new SolidColorBrush(Color.FromArgb(30, 244, 67, 54));
                    Button1.Background = new SolidColorBrush(Color.FromArgb(255, 244, 67, 54));
                    break;

                case AlertType.Warning:
                    IconText.Text = "!";
                    IconText.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 152, 0)); // Orange
                    IconBackground.Fill = new SolidColorBrush(Color.FromArgb(30, 255, 152, 0));
                    Button1.Background = new SolidColorBrush(Color.FromArgb(255, 255, 152, 0));
                    break;

                case AlertType.Info:
                default:
                    IconText.Text = "ⓘ";
                    IconText.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204)); // Blue
                    IconBackground.Fill = new SolidColorBrush(Color.FromArgb(30, 0, 122, 204));
                    Button1.Background = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = sender as FrameworkElement;

            if (button == Button1)
            {
                Result = AlertWindowResult.Yes;
            }
            else if (button == Button2)
            {
                Result = AlertWindowResult.No;
            }

            this.Close();
        }
    }

    public enum AlertWindowResult
    {
        None,
        Yes,
        No
    }
}
