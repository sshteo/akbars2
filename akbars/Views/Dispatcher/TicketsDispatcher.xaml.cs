using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using akbars.Models;
using akbars.Services;
using akbars.Views.Worker;

namespace akbars.Views.Dispatcher
{
    public partial class TicketsDispatcher : Window
    {
        private readonly SessionContext _session;

        public TicketsDispatcher(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            LoadFilters();
            LoadTickets();
        }

        private void LoadFilters()
        {
            var statuses = new List<string> { "Все" };
            statuses.AddRange(AppServices.LookupService.GetStatuses().Select(item => item.Name));
            StatusFilter.ItemsSource = statuses;
            StatusFilter.SelectedIndex = 0;

            var priorities = new List<string> { "Все" };
            priorities.AddRange(AppServices.LookupService.GetPriorities().Select(item => item.Name));
            PriorityFilter.ItemsSource = priorities;
            PriorityFilter.SelectedIndex = 0;
        }

        private void LoadTickets()
        {
            TicketsGrid.ItemsSource = AppServices.TicketService.GetTickets(new TicketQuery
            {
                StatusName = SelectedValue(StatusFilter),
                PriorityName = SelectedValue(PriorityFilter),
                SearchText = SearchBox.Text
            });
        }

        private string SelectedValue(ComboBox comboBox)
        {
            var value = comboBox.SelectedItem as string;
            return string.IsNullOrWhiteSpace(value) || value == "Все" ? null : value;
        }

        private int GetTicketId(object sender)
        {
            var frameworkElement = sender as FrameworkElement;
            return frameworkElement == null ? 0 : Convert.ToInt32(frameworkElement.Tag);
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            LoadTickets();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTickets();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var ticketId = GetTicketId(sender);
            var details = AppServices.TicketService.GetTicketDetails(ticketId);
            new DetailsTicketWorker(_session, details, false) { Owner = this }.ShowDialog();
        }

        private void AssignExecutor_Click(object sender, RoutedEventArgs e)
        {
            var ticketId = GetTicketId(sender);
            var assignWindow = new AssignExecutorWindow(ticketId, _session) { Owner = this };
            assignWindow.ShowDialog();
            LoadTickets();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
