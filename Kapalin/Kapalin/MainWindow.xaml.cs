using System.Windows;
using CobaHW7.Data;
using CobaHW7.Class;
using CobaHW7;

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
            string username = NameTextBox.Text;
            string password = PasswordBox.Password;

            var repo = new UserRepository();
            var user = await repo.GetByUsernameAsync(username);
            if (user == null)
            {
                MessageBox.Show("User tidak ditemukan");
                return;
            }

            if (!repo.VerifyPassword(user, password))
            {
                MessageBox.Show("Password salah");
                return;
            }

            if (user.IsAdmin)
            {
                var win = new DashboardAdmin();
                win.Show();
            }
            else
            {
                var win = new Dashboard(user);
                win.Show();
            }
            this.Close();
        }

        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterWindow();
            reg.Show();
            this.Close();
        }
    }
}
