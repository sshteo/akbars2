using akbars.Models;
using Npgsql;
using System.Windows;

namespace akbars.Views.Dispatcher
{
    public partial class MainDispatcher : Window
    {
        private readonly int currentUserId;  // ← добавили поле

        // Удалить или закомментировать:
        // private readonly int currentUserId;

        // И конструктор вернуть к старому виду (7 параметров):
        public MainDispatcher(
            string lastName,
            string firstName,
            string middleName,
            string email,
            string phone,
            string department,
            string role)
        {
            InitializeComponent();
            HelloText.Text = $"Привет, диспетчер {lastName} {firstName}!";
            this.Loaded += MainDispatcher_Loaded;
        }

        private void MainDispatcher_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTicketStatistics();
        }

        private void LoadTicketStatistics()
        {
            var db = new Data.Database();
            using (var conn = db.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT status_id, COUNT(*)
                    FROM tickets
                    GROUP BY status_id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteReader();
                    int newCount = 0;
                    int inProgressCount = 0;
                    int completedCount = 0;
                    int cancelledCount = 0;
                    while (reader.Read())
                    {
                        int statusId = reader.GetInt32(0);
                        int count = reader.GetInt32(1);
                        switch (statusId)
                        {
                            case 1: newCount = count; break;
                            case 2: inProgressCount = count; break;
                            case 3: completedCount = count; break;
                            case 4: cancelledCount = count; break;
                        }
                    }
                    NewTicketsText.Text = newCount.ToString();
                    InProgressTicketsText.Text = inProgressCount.ToString();
                    CompletedTicketsText.Text = completedCount.ToString();
                    CancelledTicketsText.Text = cancelledCount.ToString();
                }
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            var window = new EditProfileDispetcher(currentUserId);
            window.ShowDialog();
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {

            EmployeesDispetcher employeesDispetcher = new EmployeesDispetcher();
            employeesDispetcher.Show();
            
            
        }

        private void Dispatchers_Click(object sender, RoutedEventArgs e)
        {
            // TODO: открыть окно диспетчеров
        }

        private void Workers_Click(object sender, RoutedEventArgs e)
        {
            WorkersDispetcher workersDispetcher = new WorkersDispetcher();
            workersDispetcher.Show();
          
        }

        private void AssignTickets_Click(object sender, RoutedEventArgs e)
        {
            var ticketsDispatcherWindow = new TicketsDispatcher();  // ← без аргументов
            ticketsDispatcherWindow.Show();
            
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Main mainWindow = new Main();
            mainWindow.Show();
            this.Close();
        }
    }
}