using System.Windows;
using akbars.Services;

namespace akbars.Views
{
    public partial class Main : Window
    {
        private bool _systemAvailable;

        public Main()
        {
            InitializeComponent();
            LoginBox.Text = AppServices.Settings.RememberLogin ? AppServices.Settings.RememberedLogin : string.Empty;
            RememberMeCheck.IsChecked = AppServices.Settings.RememberLogin;
            CheckAvailability();
        }

        private void CheckAvailability()
        {
            string errorMessage;
            _systemAvailable = AppServices.AuthService.CanConnect(out errorMessage);
            LoginButton.IsEnabled = _systemAvailable;
            StatusText.Text = _systemAvailable
                ? "База данных доступна. Можно входить в систему."
                : errorMessage;
            StatusText.Foreground = _systemAvailable
                ? FindResource("AccentBrush") as System.Windows.Media.Brush
                : FindResource("DangerBrush") as System.Windows.Media.Brush;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!_systemAvailable)
            {
                MessageBox.Show(StatusText.Text, "Система недоступна", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = AppServices.AuthService.Authenticate(LoginBox.Text, PasswordBox.Password);
            if (!result.Success)
            {
                StatusText.Text = result.ErrorMessage;
                StatusText.Foreground = FindResource("DangerBrush") as System.Windows.Media.Brush;
                MessageBox.Show(result.ErrorMessage, "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppServices.Settings.SaveRememberedLogin(LoginBox.Text.Trim(), RememberMeCheck.IsChecked == true);
            AppServices.CurrentSession = result.Session;

            var dashboard = WindowFactory.CreateDashboard(result.Session);
            if (dashboard == null)
            {
                MessageBox.Show("Для этой роли пока не настроен экран.", "Неизвестная роль", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            dashboard.Show();
            Close();
        }
    }
