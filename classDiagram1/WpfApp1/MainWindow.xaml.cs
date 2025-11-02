using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Ambil nilai
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

        // TODO: Logika validasi dan registrasi
        MessageBox.Show($"Registrasi untuk: {name} ({email})");
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Buka window login
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();

            // Tutup window register ini
            this.Close();
        }
    }
}