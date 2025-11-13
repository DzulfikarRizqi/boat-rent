<<<<<<< HEAD
﻿using System.Windows;
using System;
using System.Collections.Generic;
using CobaHW7.Class;
using UserModel = CobaHW7.Class.User;
using Supabase.Gotrue;
using CobaHW7.Supabase;
using Supabase;
=======
﻿using System.Data;
using System.Windows;
using CobaHW7.Class;
using CobaHW7.Data;
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced

namespace CobaHW7
{
    public partial class RegisterWindow : Window
    {
<<<<<<< HEAD
=======
        private readonly UserRepository _repo = new UserRepository();

>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
        public RegisterWindow()
        {
            InitializeComponent();
        }

<<<<<<< HEAD
        
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
=======
        // Pastikan tombol di XAML:  Click="SignUpButton_Click"
        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text?.Trim();
            string email = EmailTextBox.Text?.Trim();
            string password = PasswordBox.Password;
            string confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                DialogService.Info("Form belum lengkap", "Nama, email, dan password wajib diisi.");
                return;
            }

            if (password != confirm)
            {
                DialogService.Error("Konfirmasi salah", "Password dan konfirmasi tidak sama.");
                return;
            }

            try
            {
                if (await _repo.EmailExistsAsync(email))
                {
                    DialogService.Error("Akun sudah teregistrasi",
                        "Email ini sudah digunakan. Silakan login atau pakai email lain.");
                    return;
                }

                var id = await _repo.CreateAsync(new User
                {
                    Name = name,
                    Email = email,
                    Password = password,   // TODO: ganti ke hash
                    IsAdmin = false
                });

                DialogService.Success("Registrasi berhasil", $"Akunmu aktif. ID: {id}");
                new MainWindow { Owner = this }.Show(); // atau LoginWindow kalau ada
                Close();
            }
            catch (DuplicateNameException)
            {
                DialogService.Error("Akun sudah teregistrasi",
                    "Email ini sudah digunakan. Silakan login atau ubah email.");
            }
            catch (Exception ex)
            {
                DialogService.Error("Gagal mendaftar", $"Terjadi kesalahan: {ex.Message}");
>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
            }
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            //new MainWindow().Show();
            //this.Close();
            MainWindow loginWindow = new MainWindow(); // Ganti dengan nama window login Anda
            loginWindow.Show();
            this.Close();
        }
=======
            new MainWindow { Owner = this }.Show();
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Owner != null) Owner.Show();
        }


>>>>>>> 9e92305cab807394a9274ee994589271c4f34ced
    }
}
