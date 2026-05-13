using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using akbars.Models;
using akbars.Services;

namespace akbars.Views.Worker
{
    public partial class TicketsWorker : Window
    {
        private readonly SessionContext _session;

        public TicketsWorker(SessionContext session)
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
                AuthorId = _session.UserId,
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

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadTickets();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTickets();
        }

        private void OpenDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button == null)
            {
                return;
            }

            var ticketId = Convert.ToInt32(button.Tag);
            var details = AppServices.TicketService.GetTicketDetails(ticketId);
            var window = new DetailsTicketWorker(_session, details, false);
            window.Owner = this;
            window.ShowDialog();
            LoadTickets();
        }

        private void CreateTicket_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new CreateTicketWindow(_session) { Owner = this };
            createWindow.ShowDialog();
            LoadTickets();
        }

        private void DeleteTicket_Click(object sender, RoutedEventArgs e)
        {
            var selected = TicketsGrid.SelectedItem as TicketListItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите заявку для удаления.", "Нет выбора", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!AppServices.TicketService.DeleteTicket(selected.Id, _session.UserId))
            {
                MessageBox.Show("Удалить можно только неназначенную заявку, созданную вами.", "Ограничение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoadTickets();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
