using System.Windows;
using CobaHW7.Class;
using CobaHW7.Data;

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
            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Semua field harus diisi.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password dan konfirmasi tidak sama.");
                return;
            }

            var repo = new UserRepository();
            var u = new User
            {
                Name = name,
                Email = email,
                Password = password,
                IsAdmin = false
            };

            var id = await repo.CreateAsync(u);
            MessageBox.Show($"Registrasi berhasil, id = {id}");
            new MainWindow().Show();
            this.Close();
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}
