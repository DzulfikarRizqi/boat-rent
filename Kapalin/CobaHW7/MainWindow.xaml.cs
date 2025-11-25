using System;
using System.Linq;
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

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                var alert = new AlertWindow("Validasi Gagal", "Email dan password tidak boleh kosong.", AlertWindow.AlertType.Warning);
                alert.ShowDialog();
                return;
            }

            SignInButton.IsEnabled = false;
            SignInButton.Content = "Signing In...";

            try
            {
                var session = await SupabaseService.Client.Auth.SignIn(email, password);

                if (session != null)
                {
                    var alert = new AlertWindow("Login Berhasil!", $"Selamat datang, {session.User.Email}!", AlertWindow.AlertType.Success);
                    alert.ShowDialog();

                    try
                    {
                        var allUsers = await SupabaseService.Client
                            .From<User>()
                            .Get();

                        var user = allUsers.Models.FirstOrDefault(u =>
                            u.Email != null && u.Email.Equals(session.User.Email, StringComparison.OrdinalIgnoreCase));

                        if (user != null)
                        {
                            if (user.IsAdmin)
                            {
                                DashboardAdmin dashboardAdmin = new DashboardAdmin();
                                dashboardAdmin.Show();
                            }
                            else
                            {
                                Dashboard dashboard = new Dashboard();
                                dashboard.Show();
                            }
                        }
                        else
                        {
                            Dashboard dashboard = new Dashboard();
                            dashboard.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        Dashboard dashboard = new Dashboard();
                        dashboard.Show();
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                var alert = new AlertWindow("Login Gagal", "Email atau Password salah!", AlertWindow.AlertType.Error);
                alert.ShowDialog();
            }
            finally
            {
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
