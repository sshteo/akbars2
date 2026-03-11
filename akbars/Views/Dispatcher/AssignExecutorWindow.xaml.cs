using Npgsql;
using System;
using System.Windows;
using System.Windows.Controls;

namespace akbars.Views.Dispatcher
{
    public partial class AssignExecutorWindow : Window
    {
        private readonly int ticketId;

        public AssignExecutorWindow(int ticketId)
        {
            InitializeComponent();
            this.ticketId = ticketId;
            LoadExecutors();
        }

        private void LoadExecutors()
        {
            ExecutorCombo.Items.Clear();
            ExecutorCombo.Items.Add("Выберите исполнителя");

            var db = new Data.Database();
            using (var conn = db.GetConnection())
            {
                conn.Open();

                const string sql = @"
                    SELECT id, 
                           first_name || ' ' || last_name AS full_name 
                    FROM users 
                    WHERE role_id = 2
                    ORDER BY last_name, first_name";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string fullName = reader.GetString(1);

                        var executorItem = new ExecutorItem
                        {
                            Id = id,
                            DisplayName = fullName + " (ID: " + id + ")"
                        };

                        ExecutorCombo.Items.Add(executorItem);
                    }
                }
            }

            ExecutorCombo.SelectedIndex = 0;
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            if (ExecutorCombo.SelectedIndex <= 0)
            {
                MessageBox.Show("Выберите исполнителя");
                return;
            }

            var selectedItem = ExecutorCombo.SelectedItem as ExecutorItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Выберите исполнителя");
                return;
            }

            int executorId = selectedItem.Id;

            try
            {
                var db = new Data.Database();
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    const string sql = @"
                        UPDATE tickets 
                        SET executor_id = @executor,
                            status_id = 2,
                            updated_at = NOW()
                        WHERE id = @ticket";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("executor", executorId);
                        cmd.Parameters.AddWithValue("ticket", ticketId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Исполнитель успешно назначен!");
                            DialogResult = true;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить заявку");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка:\n" + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ExecutorItem
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName ?? "";
        }
    }
}