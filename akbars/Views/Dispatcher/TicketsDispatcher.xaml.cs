using akbars.Views.Dispatcher; // Для AssignExecutorWindow
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System;

namespace akbars.Views.Dispatcher
{
    public partial class TicketsDispatcher : Window
    {
        private int currentUserId;

        // Было:
        // public TicketsDispatcher(int ticketId, int userId)

        // Становится:
        public TicketsDispatcher()   // ← без параметров
        {
            InitializeComponent();
            // currentUserId = userId;   ← закомментируй или удали эту строку
            LoadFilters();
            LoadTickets();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTickets();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            var ticket = (TicketView)TicketsGrid.SelectedItem;
            ShowTicketDetails(ticket.Id);
        }

       

        private void AssignExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            var ticket = (TicketView)TicketsGrid.SelectedItem;
            var assignWindow = new AssignExecutorWindow(ticket.Id);
            assignWindow.ShowDialog();
            LoadTickets();
        }

        private void ShowTicketDetails(int ticketId)
        {
            try
            {
                var db = new Data.Database();
                using (var conn = db.GetConnection())
                {
                    conn.Open();
                    string sql = @"
    SELECT
        t.id, t.short_description, t.detailed_description AS description, t.created_at, t.updated_at,
        s.name as status, p.name as priority, tt.name as type,
        u1.first_name || ' ' || u1.last_name as author,
        u2.first_name || ' ' || u2.last_name as executor,
        t.completed_at, t.assignee_id          -- ← изменил здесь
    FROM tickets t
    LEFT JOIN statuses s ON t.status_id = s.id
    LEFT JOIN priorities p ON t.priority_id = p.id
    LEFT JOIN ticket_types tt ON t.type_id = tt.id
    LEFT JOIN users u1 ON t.author_id = u1.id
    LEFT JOIN users u2 ON t.assignee_id = u2.id   -- ← и здесь
    WHERE t.id = @id";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("id", ticketId);
                        var reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string details = $"📋 Информация о заявке #{reader.GetInt32(0)}\n\n";
                            details += $"📝 Краткое описание: {reader.GetString(1)}\n";
                            details += $"📄 Полное описание: {reader.GetString(2)}\n\n";
                            details += $"📅 Создана: {reader.GetDateTime(3):dd.MM.yyyy HH:mm}\n";
                            details += $"✏️ Последнее изменение: {reader.GetDateTime(4):dd.MM.yyyy HH:mm}\n\n";

                            string status = reader.IsDBNull(5) ? "Не указан" : reader.GetString(5);
                            details += $"🏷 Статус: {status}\n";

                            string priority = reader.IsDBNull(6) ? "Не указан" : reader.GetString(6);
                            details += $"⭐ Приоритет: {priority}\n";

                            string type = reader.IsDBNull(7) ? "Не указан" : reader.GetString(7);
                            details += $"📂 Тип: {type}\n\n";

                            string author = reader.IsDBNull(8) ? "Неизвестно" : reader.GetString(8);
                            details += $"👤 Создатель: {author}\n";

                            string executor = reader.IsDBNull(10) ? "Не назначен" : reader.GetString(9);
                            details += $"🔧 Исполнитель: {executor}\n";

                            if (!reader.IsDBNull(10) && !reader.IsDBNull(11))
                                details += $"✅ Завершена: {reader.GetDateTime(10):dd.MM.yyyy HH:mm}";

                            MessageBox.Show(details, "Детали заявки", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadFilters()
        {
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                StatusFilter.Items.Add("Все");

                var statusCmd = new NpgsqlCommand("SELECT name FROM statuses", conn);
                var statusReader = statusCmd.ExecuteReader();

                while (statusReader.Read())
                    StatusFilter.Items.Add(statusReader.GetString(0));

                statusReader.Close();

                PriorityFilter.Items.Add("Все");

                var priorityCmd = new NpgsqlCommand("SELECT name FROM priorities", conn);
                var priorityReader = priorityCmd.ExecuteReader();

                while (priorityReader.Read())
                    PriorityFilter.Items.Add(priorityReader.GetString(0));
            }

            StatusFilter.SelectedIndex = 0;
            PriorityFilter.SelectedIndex = 0;
        }

        private void LoadTickets()
        {
            var tickets = new List<TicketView>();

            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"
SELECT 
t.id,
t.created_at,
t.updated_at,
t.short_description,
tt.name,
p.name,
s.name
FROM tickets t
LEFT JOIN ticket_types tt ON t.type_id = tt.id
LEFT JOIN priorities p ON t.priority_id = p.id
LEFT JOIN statuses s ON t.status_id = s.id
WHERE 1=1";

                string selectedStatus = StatusFilter.SelectedItem?.ToString();
                string selectedPriority = PriorityFilter.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
                    sql += " AND s.name = @status";

                if (!string.IsNullOrEmpty(selectedPriority) && selectedPriority != "Все")
                    sql += " AND p.name = @priority";

                var cmd = new NpgsqlCommand(sql, conn);

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
                    cmd.Parameters.AddWithValue("status", selectedStatus);

                if (!string.IsNullOrEmpty(selectedPriority) && selectedPriority != "Все")
                    cmd.Parameters.AddWithValue("priority", selectedPriority);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tickets.Add(new TicketView
                    {
                        Id = reader.GetInt32(0),
                        CreatedAt = reader.GetDateTime(1).ToString("dd.MM.yyyy HH:mm"),
                        UpdatedAt = reader.GetDateTime(2).ToString("dd.MM.yyyy HH:mm"),
                        Description = reader.GetString(3),
                        Type = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        Priority = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        Status = reader.IsDBNull(6) ? "" : reader.GetString(6)
                    });
                }
            }

            TicketsGrid.ItemsSource = tickets;
        }

        private void Back_Click(object sender, RoutedEventArgs e) 
        {
           
            this.Close();

        }

        private void CreateTicket_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void DeleteTicket_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            var ticket = (TicketView)TicketsGrid.SelectedItem;

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить заявку?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM tickets WHERE id=@id";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", ticket.Id);
               

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Заявка удалена");

            LoadTickets();
        }
    }

    public class TicketView
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
