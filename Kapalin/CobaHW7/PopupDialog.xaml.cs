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
    public partial class PopupDialog : Window
    {
        public PopupDialog(string title, string message, DialogKind kind)
        {
            InitializeComponent();
            TitleBlock.Text = title;
            MessageBlock.Text = message;

            switch (kind)
            {
                case DialogKind.Success:
                    AccentBar.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#00C853"),
                        (Color)ColorConverter.ConvertFromString("#00E676"), 0);
                    IconBlock.Text = "\xE73E"; // CheckMark
                    IconBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C853"));
                    break;

                case DialogKind.Error:
                    AccentBar.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#D32F2F"),
                        (Color)ColorConverter.ConvertFromString("#FF5252"), 0);
                    IconBlock.Text = "\xE783"; // ErrorBadge
                    IconBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
                    break;

                default: // Info
                    AccentBar.Background = new LinearGradientBrush(
                        (Color)ColorConverter.ConvertFromString("#2979FF"),
                        (Color)ColorConverter.ConvertFromString("#00B0FF"), 0);
                    IconBlock.Text = "\xE946"; // Info
                    IconBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2979FF"));
                    break;
            }

           
            WindowStartupLocation = Owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
        }

        private void Ok_Click(object sender, RoutedEventArgs e) => Close();
    }
}
