using System;
using System.Windows;
using akbars.Models;
using akbars.Services;

namespace akbars.Views.Worker
{
    public partial class CreateTicketWindow : Window
    {
        private readonly SessionContext _session;

        public CreateTicketWindow(SessionContext session)
        {
            InitializeComponent();
            _session = session;
            LoadLookups();
        }

        private void LoadLookups()
        {
            TypeBox.ItemsSource = AppServices.LookupService.GetTicketTypes();
            PriorityBox.ItemsSource = AppServices.LookupService.GetPriorities();

            if (TypeBox.Items.Count > 0)
            {
                TypeBox.SelectedIndex = 0;
            }

            if (PriorityBox.Items.Count > 0)
            {
                PriorityBox.SelectedIndex = 0;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var priority = PriorityBox.SelectedItem as Priority;
                var type = TypeBox.SelectedItem as TicketType;

                AppServices.TicketService.CreateTicket(
                    _session.UserId,
                    ShortDescriptionBox.Text,
                    DetailedDescriptionBox.Text,
                    priority == null ? 0 : priority.Id,
                    type == null ? 0 : type.Id);

                MessageBox.Show("Заявка создана.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Не удалось создать заявку", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
