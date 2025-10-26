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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Ambil nilai dari TextBox dan PasswordBox
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            // 2. Lakukan validasi (Contoh sederhana)
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email dan password tidak boleh kosong.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. (TODO) Cek ke database atau ke kelas User Anda
            // Contoh:
            // User user = new User();
            // if (user.login(email, password))
            // {
            //     MessageBox.Show("Login Berhasil!", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            //     // TODO: Buka window utama aplikasi Anda di sini
            //     // var mainAppWindow = new MainAppWindow();
            //     // mainAppWindow.Show();
            //     // this.Close();
            // }
            // else
            // {
            //     MessageBox.Show("Email atau password salah.", "Gagal Login", MessageBoxButton.OK, MessageBoxImage.Error);
            // }

            // Hapus ini jika Anda sudah implementasi logika di atas
            MessageBox.Show($"Email: {email}\nPassword: (disembunyikan)", "Login Ditekan");
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Buat instance dari RegisterWindow
            RegisterWindow registerWindow = new RegisterWindow();

            // Tampilkan window register
            registerWindow.Show();

            // Tutup window login saat ini
            this.Close();
        }
    }
}