using System.Data;
using System.Windows;
using CobaHW7.Class;
using CobaHW7.Data;

namespace CobaHW7
{
    public partial class RegisterWindow : Window
    {
        private readonly UserRepository _repo = new UserRepository();

        public RegisterWindow()
        {
            InitializeComponent();
        }

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
            }
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow { Owner = this }.Show();
            Close();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Owner != null) Owner.Show();
        }


    }
}
