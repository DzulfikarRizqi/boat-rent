using System.Windows;
using System;
using System.Collections.Generic;
using CobaHW7.Class;
using UserModel = CobaHW7.Class.User;
using Supabase.Gotrue;
using CobaHW7.Services;
using Supabase;

namespace CobaHW7
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }


        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text;
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            // Validasi sederhana
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                var alert = new AlertWindow("Validasi Gagal", "Nama, email, dan password tidak boleh kosong.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            if (password != confirmPassword)
            {
                var alert = new AlertWindow("Validasi Gagal", "Password dan Konfirmasi Password tidak cocok.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            // Nonaktifkan tombol untuk mencegah klik ganda
            SignUpButton.IsEnabled = false;
            SignUpButton.Content = "Mendaftar...";

            try
            {
                // Panggil Supabase untuk mendaftarkan pengguna (Authentication)
                var session = await SupabaseService.Client.Auth.SignUp(email, password);

                // Jika pendaftaran Auth berhasil, `session` tidak akan null
                if (session != null)
                {
                    var alert = new AlertWindow("Pendaftaran Berhasil!", "Akun Anda telah dibuat. Silakan login dengan email dan password Anda.", AlertWindow.AlertType.Success);
                    alert.ShowDialog();

                    // Arahkan ke halaman login
                    MainWindow loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Tangani error
                var alert = new AlertWindow("Pendaftaran Gagal", $"Terjadi kesalahan: {ex.Message}", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
            finally
            {
                // Aktifkan kembali tombol setelah proses selesai
                SignUpButton.IsEnabled = true;
                SignUpButton.Content = "Sign Up";
            }
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            //new MainWindow().Show();
            //this.Close();
            MainWindow loginWindow = new MainWindow(); // Ganti dengan nama window login Anda
            loginWindow.Show();
            this.Close();
        }
    }
}
