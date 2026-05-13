using System;
using System.Linq;
using System.Windows;
using akbars.Models;
using akbars.Services;

namespace akbars.Views.Dispatcher
{
    public partial class AssignExecutorWindow : Window
    {
        private readonly int _ticketId;
        private readonly SessionContext _session;

        public AssignExecutorWindow(int ticketId, SessionContext session)
        {
            InitializeComponent();
            _ticketId = ticketId;
            _session = session;
            LoadExecutors();
        }

        private void LoadExecutors()
        {
            ExecutorCombo.ItemsSource = AppServices.UserService.GetUsers(2)
                .OrderBy(user => user.LastName)
                .ToList();

            if (ExecutorCombo.Items.Count > 0)
            {
                ExecutorCombo.SelectedIndex = 0;
            }
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            var user = ExecutorCombo.SelectedItem as User;
            if (user == null)
            {
                MessageBox.Show("Выберите исполнителя.", "Нет выбора", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                AppServices.TicketService.AssignTicket(_ticketId, user.Id, _session.UserId);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось назначить исполнителя: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
