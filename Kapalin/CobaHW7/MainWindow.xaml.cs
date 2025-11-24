using System.Windows;
using CobaHW7.Class;
using CobaHW7;
using CobaHW7.Services;

namespace CobaHW7
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;

            // Validasi input sederhana
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                var alert = new AlertWindow("Validasi Gagal", "Email dan password tidak boleh kosong.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            if (email == "admin@gmail.com" && password == "admin")
            {
                DashboardAdmin dashboardAdmin = new DashboardAdmin();
                dashboardAdmin.Show();
                this.Close();
            }

            // Beri umpan balik ke UI
            SignInButton.IsEnabled = false;
            SignInButton.Content = "Signing In...";

            try
            {
                // Panggil Supabase.Auth.SignIn
                var session = await SupabaseService.Client.Auth.SignIn(email, password);

                // Tangani login berhasil
                if (session != null)
                {
                    var alert = new AlertWindow("Login Berhasil!", $"Selamat datang, {session.User.Email}!", AlertWindow.AlertType.Success);
                    alert.ShowDialog();

                    Dashboard dashboard = new Dashboard();
                    dashboard.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Tangani error
                var alert = new AlertWindow("Login Gagal", "Email atau Password salah!", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
            finally
            {
                // Kembalikan UI ke normal
                SignInButton.IsEnabled = true;
                SignInButton.Content = "Sign In";
            }
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            //var reg = new RegisterWindow();
            //reg.Show();
            //this.Close();

            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}
