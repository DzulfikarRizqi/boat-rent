using System;
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

                    // Ambil data user dari tabel users di Supabase
                    try
                    {
                        var userList = await SupabaseService.Client
                            .From<User>()
                            .Where(u => u.Email == session.User.Email)
                            .Get();

                        if (userList.Models.Count > 0)
                        {
                            var user = userList.Models[0];

                            // Cek apakah user adalah admin (is_admin = 1 atau true)
                            if (user.IsAdmin)
                            {
                                // Buka DashboardAdmin
                                DashboardAdmin dashboardAdmin = new DashboardAdmin();
                                dashboardAdmin.Show();
                            }
                            else
                            {
                                // Buka Dashboard regular user
                                Dashboard dashboard = new Dashboard();
                                dashboard.Show();
                            }
                        }
                        else
                        {
                            // User tidak ditemukan di tabel users, buka dashboard regular
                            Dashboard dashboard = new Dashboard();
                            dashboard.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Jika ada error saat mengambil data user, default ke Dashboard regular
                        Dashboard dashboard = new Dashboard();
                        dashboard.Show();
                    }

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
