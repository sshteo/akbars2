using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using akbars.Services;

namespace akbars.Views.Dispatcher
{
    public partial class EmployeesDispetcher : Window
    {
        public EmployeesDispetcher()
        {
            InitializeComponent();
            EmployeesGrid.ItemsSource = AppServices.UserService.GetUsers(1).OrderBy(user => user.LastName).ToList();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
            {
                return;
            }

            var user = AppServices.UserService.GetUser(Convert.ToInt32(element.Tag));
            if (user == null)
            {
                return;
            }

            MessageBox.Show(
                string.Format(
                    "ФИО: {0}\nEmail: {1}\nТелефон: {2}\nОтдел: {3}\nРоль: {4}",
                    user.FullName,
                    string.IsNullOrWhiteSpace(user.Email) ? "не указан" : user.Email,
                    string.IsNullOrWhiteSpace(user.Phone) ? "не указан" : user.Phone,
                    string.IsNullOrWhiteSpace(user.Department) ? "не указан" : user.Department,
                    user.RoleName),
                "Карточка сотрудника",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
