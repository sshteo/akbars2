using System;
using System.Windows;
using akbars.Models;
using akbars.Services;
using akbars.Views;
using akbars.Views.Dispatcher;
using akbars.Views.Worker;

namespace akbars.Views.Repairman
{
    public partial class MainRepairman : Window
    {
        private readonly SessionContext _session;

        public MainRepairman(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            RefreshDashboard();
        }

        private void RefreshDashboard()
        {
            var stats = AppServices.TicketService.GetStatistics(null, _session.UserId);
            HelloText.Text = "Здравствуйте, " + _session.FirstName + ".";
            AssignedText.Text = stats.Total.ToString();
            InProgressText.Text = stats.InProgressCount.ToString();
            CompletedText.Text = stats.CompletedCount.ToString();
            OverdueText.Text = stats.OverdueCount.ToString();
            TicketsGrid.ItemsSource = AppServices.TicketService.GetTickets(new TicketQuery { AssigneeId = _session.UserId });
        }

        private void OpenTicket_Click(object sender, RoutedEventArgs e)
        {
            var ticketId = Convert.ToInt32((sender as FrameworkElement).Tag);
            var details = AppServices.TicketService.GetTicketDetails(ticketId);
            new DetailsTicketWorker(_session, details, true) { Owner = this }.ShowDialog();
            RefreshDashboard();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            new EditProfileDispetcher(_session) { Owner = this }.ShowDialog();
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
