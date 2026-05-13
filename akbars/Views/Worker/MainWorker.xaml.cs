using System.Windows;
using akbars.Models;
using akbars.Services;
using akbars.Views;
using akbars.Views.Dispatcher;

namespace akbars.Views.Worker
{
    public partial class MainWorker : Window
    {
        private readonly SessionContext _session;

        public MainWorker(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            var stats = AppServices.TicketService.GetStatistics(_session.UserId, null);

            HelloText.Text = "Здравствуйте, " + _session.FirstName + ".";
            RoleHintText.Text = "Ваш контур: создание и отслеживание сервисных заявок.";
            FullNameText.Text = _session.FullName;
            EmailText.Text = _session.Email;
            PhoneText.Text = string.IsNullOrWhiteSpace(_session.Phone) ? "Не указан" : _session.Phone;
            DepartmentText.Text = string.IsNullOrWhiteSpace(_session.Department) ? "Не указан" : _session.Department;
            RoleText.Text = _session.RoleName;

            TotalTicketsText.Text = stats.Total.ToString();
            NewTicketsText.Text = stats.NewCount.ToString();
            InProgressTicketsText.Text = stats.InProgressCount.ToString();
            CompletedTicketsText.Text = stats.CompletedCount.ToString();
        }

        private void Tickets_Click(object sender, RoutedEventArgs e)
        {
            new TicketsWorker(_session).ShowDialog();
            RefreshDashboard();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var editor = new EditProfileDispetcher(_session);
            editor.Owner = this;
            editor.ShowDialog();
            RefreshDashboard();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppServices.CurrentSession = null;
            new Main().Show();
            Close();
        }
    }
