using System;
using System.Linq;
using System.Windows;
using akbars.Models;
using akbars.Services;
using akbars.Views;

namespace akbars.Views.Admin
{
    public partial class MainAdmin : Window
    {
        private readonly SessionContext _session;

        public MainAdmin(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            RefreshAll();
        }

        private void RefreshAll()
        {
            HelloText.Text = "Здравствуйте, " + _session.FirstName + ".";

            var users = AppServices.UserService.GetUsers(null);
            var roles = AppServices.LookupService.GetRoles();
            var ticketStats = AppServices.TicketService.GetStatistics(null, null);

            UsersGrid.ItemsSource = users;
            RoleSelector.ItemsSource = roles;
            if (RoleSelector.Items.Count > 0)
            {
                RoleSelector.SelectedIndex = 0;
            }

            PrioritiesGrid.ItemsSource = AppServices.LookupService.GetPriorities();
            StatusesGrid.ItemsSource = AppServices.LookupService.GetStatuses();
            TypesGrid.ItemsSource = AppServices.LookupService.GetTicketTypes();

            UsersCountText.Text = users.Count.ToString();
            EmployeesCountText.Text = users.Count(user => user.RoleId == 1).ToString();
            ExecutorsCountText.Text = users.Count(user => user.RoleId == 2).ToString();
            TicketsCountText.Text = ticketStats.Total.ToString();
        }

        private void UpdateRole_Click(object sender, RoutedEventArgs e)
        {
            var user = UsersGrid.SelectedItem as User;
            var role = RoleSelector.SelectedItem as Role;
            if (user == null || role == null)
            {
                MessageBox.Show("Выберите пользователя и роль.", "Недостаточно данных", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppServices.UserService.UpdateRole(user.Id, role.Id);
            RefreshAll();
        }

        private void AddPriority_Click(object sender, RoutedEventArgs e)
        {
            int slaHours;
            if (!int.TryParse(PrioritySlaBox.Text, out slaHours))
            {
                MessageBox.Show("SLA должен быть числом.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AppServices.LookupService.AddPriority(PriorityNameBox.Text.Trim(), slaHours);
            PriorityNameBox.Clear();
            PrioritySlaBox.Clear();
            RefreshAll();
        }

        private void AddStatus_Click(object sender, RoutedEventArgs e)
        {
            AppServices.LookupService.AddStatus(StatusNameBox.Text.Trim(), StatusDescriptionBox.Text.Trim());
            StatusNameBox.Clear();
            StatusDescriptionBox.Clear();
            RefreshAll();
        }

        private void AddType_Click(object sender, RoutedEventArgs e)
        {
            AppServices.LookupService.AddTicketType(TypeNameBox.Text.Trim());
            TypeNameBox.Clear();
            RefreshAll();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAll();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppServices.CurrentSession = null;
            new Main().Show();
            Close();
        }
    }
