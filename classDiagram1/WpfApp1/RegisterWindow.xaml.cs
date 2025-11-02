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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Validasi sederhana
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Semua field harus diisi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password dan Konfirmasi Password tidak cocok.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // TODO: Tambahkan logika untuk mendaftarkan user (misalnya ke database)
            // User newUser = new User();
            // bool success = newUser.register(name, email, password);
            // if(success) { ... }

            MessageBox.Show("Registrasi berhasil!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

            // Setelah sukses register, arahkan ke halaman login
            LoginHyperlink_Click(sender, e);
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Buka window login
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();

            // Tutup window register ini
            this.Close();
        }
    }
}
