using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace akbars.Views.Worker
{
    public partial class TicketsWorker : Window
    {
        private int currentUserId;

        public TicketsWorker(int userId)
        {
            InitializeComponent();

            currentUserId = userId;

            LoadFilters();
            LoadTickets();
        }
/*
        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedTicket = (TicketView)TicketsGrid.SelectedItem;

            ShowMyTicketDetails(selectedTicket.Id);

            // Если хотите после просмотра обновить список — раскомментируйте:
            // LoadTickets();
        }
*/
        private void ShowMyTicketDetails(int ticketId)
        {
           /* try
            {
                var db = new Data.Database();
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    t.id,
                    t.short_description,
                    t.detailed_description,
                    t.created_at,
                    t.updated_at,
                    s.name AS status,
                    p.name AS priority,
                    tt.name AS type,
                    t.author_id,
                    u.first_name || ' ' || u.last_name AS author_name,
                    t.assignee_id,
                    e.first_name || ' ' || e.last_name AS executor_name,
                    t.completed_at
                FROM tickets t
                LEFT JOIN statuses      s  ON t.status_id   = s.id
                LEFT JOIN priorities    p  ON t.priority_id = p.id
                LEFT JOIN ticket_types  tt ON t.type_id     = tt.id
                LEFT JOIN users         u  ON t.author_id   = u.id
                LEFT JOIN users         e  ON t.assignee_id = e.id
                WHERE t.id = @id
                  AND t.author_id = @userId";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("id", ticketId);
                        cmd.Parameters.AddWithValue("userId", currentUserId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Заявка не найдена или вам не принадлежит",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            // Собираем красивый текст
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine($"📋 Заявка № {reader.GetInt32("id")}");
                            sb.AppendLine(new string('─', 40));
                            sb.AppendLine();

                            sb.AppendLine($"📝 Краткое описание:");
                            sb.AppendLine(reader.GetString("short_description"));
                            sb.AppendLine();

                            sb.AppendLine($"📄 Подробное описание:");
                            string detailed = reader.IsDBNull("detailed_description")
                                ? "— не указано —"
                                : reader.GetString("detailed_description");
                            sb.AppendLine(detailed);
                            sb.AppendLine();

                            sb.AppendLine($"📅 Создана:     {reader.GetDateTime("created_at"):dd.MM.yyyy HH:mm}");
                            sb.AppendLine($"✏️ Обновлена:    {reader.GetDateTime("updated_at"):dd.MM.yyyy HH:mm}");

                            string status = reader.IsDBNull("status") ? "—" : reader.GetString("status");
                            sb.AppendLine($"🏷 Статус:       {status}");

                            string priority = reader.IsDBNull("priority") ? "—" : reader.GetString("priority");
                            sb.AppendLine($"⭐ Приоритет:    {priority}");

                            string type = reader.IsDBNull("type") ? "—" : reader.GetString("type");
                            sb.AppendLine($"📂 Тип:          {type}");
                            sb.AppendLine();

                            sb.AppendLine($"👤 Автор:        {reader.GetString("author_name")}");

                            string executor = reader.IsDBNull("executor_name")
                                ? "не назначен"
                                : reader.GetString("executor_name");
                            sb.AppendLine($"🔧 Исполнитель:  {executor}");

                            if (!reader.IsDBNull("completed_at"))
                            {
                                sb.AppendLine();
                                sb.AppendLine($"✅ Завершена:    {reader.GetDateTime("completed_at"):dd.MM.yyyy HH:mm}");
                            }

                            MessageBox.Show(sb.ToString(),
                                "Детали моей заявки",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке деталей заявки:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           */
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
        WHERE t.author_id = @user";

                string selectedStatus = StatusFilter.SelectedItem?.ToString();
                string selectedPriority = PriorityFilter.SelectedItem?.ToString();

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Все")
                    sql += " AND s.name = @status";

                if (!string.IsNullOrEmpty(selectedPriority) && selectedPriority != "Все")
                    sql += " AND p.name = @priority";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("user", currentUserId);

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

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTickets();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateTicket_Click(object sender, RoutedEventArgs e)
        {
         var window = new CreateTicketWindow(currentUserId);
          window.ShowDialog();

            LoadTickets();
        }

        private void TicketsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // можно добавить логику при выборе заявки
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

                string sql = "DELETE FROM tickets WHERE id=@id AND author_id=@user";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", ticket.Id);
                cmd.Parameters.AddWithValue("user", currentUserId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Заявка удалена");

            LoadTickets();
        }

        // Добавь в TicketsDispatcher.cs эти методы:

        private void AssignExecutor_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку");
                return;
            }

            var ticket = (TicketView)TicketsGrid.SelectedItem;

            // Открываем окно выбора исполнителя
            var assignWindow = new akbars.Views.Dispatcher.AssignExecutorWindow(ticket.Id);
            assignWindow.ShowDialog();

            LoadTickets(); // Обновляем список
        }

       

        private void ShowTicketDetails(int ticketId)
        {
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT 
                t.id, t.short_description, t.description, t.created_at, t.updated_at,
                s.name as status, p.name as priority, tt.name as type,
                u1.first_name || ' ' || u1.last_name as author,
                e.first_name || ' ' || e.last_name as executor,
                t.completed_at,
                t.executor_id
            FROM tickets t
            LEFT JOIN statuses s ON t.status_id = s.id
            LEFT JOIN priorities p ON t.priority_id = p.id
            LEFT JOIN ticket_types tt ON t.type_id = tt.id
            LEFT JOIN users u1 ON t.author_id = u1.id
            LEFT JOIN users e ON t.executor_id = e.id
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
                        details += $"✏️ Последнее изменение: {reader.GetDateTime(4):dd.MM.yyyy HH:mm}\n";

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

                        if (!reader.IsDBNull(11))
                            details += $"✅ Завершена: {reader.GetDateTime(11):dd.MM.yyyy HH:mm}";

                        MessageBox.Show(details, "Детали заявки", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

    };
    

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