using System.Windows;
using CobaHW7.Class;
using CobaHW7;
using CobaHW7.Supabase;

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

            // 2. Validasi input sederhana
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email dan password tidak boleh kosong.", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (email == "admin@gmail.com" && password == "admin")
            {
                DashboardAdmin dashboardAdmin = new DashboardAdmin();
                dashboardAdmin.Show();
                this.Close();
            }

            // 3. Beri umpan balik ke UI
            SignInButton.IsEnabled = false;
            SignInButton.Content = "Signing In...";

            try
            {
                // 4. Panggil Supabase.Auth.SignIn
                var session = await SupabaseService.Client.Auth.SignIn(email, password);

                // 5. Tangani login berhasil
                if (session != null)
                {
                    MessageBox.Show($"Login berhasil! Selamat datang, {session.User.Email}.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                    // TODO: Buka Dashboard Anda
                    Dashboard dashboard = new Dashboard();
                    dashboard.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // 6. Tangani error (mis. "Invalid login credentials")
                MessageBox.Show($"Login gagal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 7. Kembalikan UI ke normal
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
