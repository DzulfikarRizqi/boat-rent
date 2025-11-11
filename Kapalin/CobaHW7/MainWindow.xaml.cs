using System;
using System.Windows;
using System.Windows.Input;   // untuk enter-to-login
using CobaHW7.Data;

namespace CobaHW7
{
    public partial class MainWindow : Window
    {
        private readonly UserRepository _repo = new UserRepository();

        public MainWindow()
        {
            InitializeComponent();
        }

        // ========= NAVIGASI KE REGISTER =========
        // XAML: <Hyperlink Click="RegisterHyperlink_Click">Register</Hyperlink>
        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reg = new RegisterWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                // Setelah Register ditutup, munculkan login lagi
                reg.Closed += (_, __) =>
                {
                    if (!IsVisible) this.Show();
                };

                this.Hide();   // sembunyikan login biar app tidak exit
                reg.Show();
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal membuka Register", ex.ToString());
            }
        }

        // ========= LOGIN =========
        // XAML: <Button Click="SignInButton_Click" .../>
        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ambil identifier: pakai EmailTextBox kalau diisi; kalau kosong, NameTextBox
                var ident = !string.IsNullOrWhiteSpace(EmailTextBox.Text)
                              ? EmailTextBox.Text.Trim()
                              : (NameTextBox.Text ?? "").Trim();

                var pass = (PasswordBox.Password ?? "").Trim();

                if (string.IsNullOrWhiteSpace(ident) || string.IsNullOrWhiteSpace(pass))
                {
                    DialogService.Info("Form belum lengkap", "Isi username/email dan password.");
                    return;
                }

                // Cari user by username ATAU email (repo sudah handle OR)
                var user = await _repo.GetByLoginAsync(ident);
                if (user is null)
                {
                    DialogService.Info("Akun tidak ditemukan",
                        "Periksa lagi username/email atau lakukan registrasi.");
                    return;
                }

                if (!_repo.VerifyPassword(user, pass))
                {
                    DialogService.Error("Password salah", "Kata sandi tidak cocok. Coba lagi ya.");
                    return;
                }

                // Sukses → buka dashboard
                //DialogService.Success("Login berhasil", $"Selamat datang, {user.Name}!");

                try
                {
                    // Tentukan jendela dashboard sesuai role
                    Window dash = user.IsAdmin
                        ? new DashboardAdmin()     // sesuaikan kalau butuh parameter
                        : new Dashboard(user);     // dashboard user biasa

                    dash.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                    // PENTING: jadikan dashboard sebagai MainWindow baru
                    Application.Current.MainWindow = dash;

                    dash.Show();
                    this.Close(); // aman: MainWindow baru sudah diset
                }
                catch (Exception openEx)
                {
                    DialogService.Error("Gagal buka Dashboard", openEx.ToString());
                }
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal login", ex.ToString());
            }
        }

        // ========= ENTER-TO-LOGIN (opsional) =========
        // Hubungkan di XAML: mis. <Grid KeyDown="RootGrid_KeyDown">...</Grid>
        private void RootGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SignInButton_Click(sender, e);
            }
        }

        // Tidak perlu override OnClosed apa-apa di sini.
        // Dengan Application.Current.MainWindow = dashboard, app tetap hidup.
    }
}
