using System.Windows;
using akbars.Models;
using akbars.Services;
using akbars.Views;

namespace akbars.Views.Dispatcher
{
    public partial class MainDispatcher : Window
    {
        private readonly SessionContext _session;

        public MainDispatcher(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            var stats = AppServices.TicketService.GetStatistics(null, null);
            HelloText.Text = "Здравствуйте, " + _session.FirstName + ".";
            TotalTicketsText.Text = stats.Total.ToString();
            NewTicketsText.Text = stats.NewCount.ToString();
            InProgressTicketsText.Text = stats.InProgressCount.ToString();
            CompletedTicketsText.Text = stats.CompletedCount.ToString();
            OverdueTicketsText.Text = stats.OverdueCount.ToString();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            new EditProfileDispetcher(_session) { Owner = this }.ShowDialog();
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            new EmployeesDispetcher().ShowDialog();
        }

        private void Workers_Click(object sender, RoutedEventArgs e)
        {
            new WorkersDispetcher().ShowDialog();
        }

        private void AssignTickets_Click(object sender, RoutedEventArgs e)
        {
            new TicketsDispatcher(_session) { Owner = this }.ShowDialog();
            RefreshDashboard();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshDashboard();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppServices.CurrentSession = null;
            new Main().Show();
            Close();
        }
    }
