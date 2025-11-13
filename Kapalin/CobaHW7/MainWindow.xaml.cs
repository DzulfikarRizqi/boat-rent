<<<<<<< HEAD
﻿using System.Windows;
using CobaHW7.Class;
using CobaHW7;
using CobaHW7.Supabase;
=======
﻿using System;
using System.Windows;
using System.Windows.Input;   // untuk enter-to-login
using CobaHW7.Data;
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced

namespace CobaHW7
{
    public partial class MainWindow : Window
    {
<<<<<<< HEAD
=======
        private readonly UserRepository _repo = new UserRepository();

>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
        public MainWindow()
        {
            InitializeComponent();
        }

<<<<<<< HEAD
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
=======
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
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
                }
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
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
=======
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
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
    }
}
