using System.Windows;

namespace CobaHW7
{
    public partial class BookingConfirmationWindow : Window
    {
        public bool IsConfirmed { get; private set; } = false;

        public enum ConfirmationType
        {
            Confirm,
            Cancel,
            Success
        }

        public BookingConfirmationWindow(string title, string message, ConfirmationType type = ConfirmationType.Confirm)
        {
            InitializeComponent();
            SetupConfirmation(title, message, type);
        }

        private void SetupConfirmation(string title, string message, ConfirmationType type)
        {
            TitleText.Text = title;
            MessageText.Text = message;

            if (type == ConfirmationType.Success)
            {
                IconBackground.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 220, 252, 231));
                IconText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 34, 197, 94));
                IconText.Text = "✓";
                YesButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 34, 197, 94));
                YesButton.Content = "Oke";
                NoButton.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (type == ConfirmationType.Cancel)
            {
                IconBackground.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 235, 238));
                IconText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 229, 57, 53));
                IconText.Text = "⚠";
                YesButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 229, 57, 53));
                YesButton.Content = "Ya";
                NoButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                IconBackground.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 243, 205));
                IconText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 152, 0));
                IconText.Text = "?";
                YesButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 25, 118, 210));
                YesButton.Content = "Ya";
                NoButton.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }
    }
}
