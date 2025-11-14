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

            //var name = NameTextBox.Text;
            //var email = EmailTextBox.Text;
            //var password = PasswordBox.Password;
            //var confirmPassword = ConfirmPasswordBox.Password;

            //// 2. Lakukan validasi sederhana
            //if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            //{
            //    MessageBox.Show("Nama, email, dan password tidak boleh kosong.", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //if (password != confirmPassword)
            //{
            //    MessageBox.Show("Password dan Konfirmasi Password tidak cocok.", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //// Nonaktifkan tombol untuk mencegah klik ganda
            //SignUpButton.IsEnabled = false;
            //SignUpButton.Content = "Mendaftar...";

            //try
            //{

            //    // 3. Panggil Supabase untuk mendaftarkan pengguna (Authentication)
            //    var session = await SupabaseService.Client.Auth.SignUp(email, password);

            //    // Jika pendaftaran Auth berhasil, `session` tidak akan null
            //    if (session?.User != null)
            //    {
            //        // 4. Masukkan data profil tambahan (seperti nama) ke tabel 'users' Anda
            //        var newUserProfile = new UserModel // Menggunakan model User.cs Anda
            //        {

            //            Username = name,
            //            Email = email
            //            // Isi properti lain jika ada
            //        };

            //        await SupabaseService.Client.From<UserModel>().Insert(newUserProfile);

            //        MessageBox.Show("Pendaftaran berhasil! Silakan login.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

            //        // Arahkan ke halaman login
            //        MainWindow loginWindow = new MainWindow();
            //        loginWindow.Show();
            //        this.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Tangani error, misalnya jika email sudah terdaftar
            //    MessageBox.Show($"Pendaftaran gagal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //finally
            //{
            //    // Aktifkan kembali tombol setelah proses selesai
            //    SignUpButton.IsEnabled = true;
            //    SignUpButton.Content = "Sign Up";
            //}

            var name = NameTextBox.Text;
            var email = EmailTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

            // 2. Lakukan validasi sederhana
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Nama, email, dan password tidak boleh kosong.", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password dan Konfirmasi Password tidak cocok.", "Validasi Gagal", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Nonaktifkan tombol untuk mencegah klik ganda
            SignUpButton.IsEnabled = false;
            SignUpButton.Content = "Mendaftar...";

            try
            {

                // 3. Panggil Supabase untuk mendaftarkan pengguna (Authentication)
                var session = await SupabaseService.Client.Auth.SignUp(email, password);

                // Jika pendaftaran Auth berhasil, `session` tidak akan null
                if (session != null)
                {

                    MessageBox.Show("Pendaftaran berhasil! Silakan login.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Arahkan ke halaman login
                    MainWindow loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Tangani error, misalnya jika email sudah terdaftar
                MessageBox.Show($"Pendaftaran gagal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
