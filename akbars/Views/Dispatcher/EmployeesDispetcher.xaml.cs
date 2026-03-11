using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;


namespace akbars.Views.Dispatcher
{
    public partial class EmployeesDispetcher : Window
    {
        public EmployeesDispetcher()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var employees = new List<EmployeeView>();
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"
SELECT 
u.id,
u.last_name,
u.first_name,
u.email,
u.phone,
u.department,
r.name
FROM users u
LEFT JOIN roles r ON u.role_id = r.id
WHERE u.role_id = 1";

                var cmd = new NpgsqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new EmployeeView
                    {
                        Id = reader.GetInt32(0),
                        LastName = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        Email = reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        Department = reader.IsDBNull(5) ? "" : reader.GetString(5),
                        Role = reader.IsDBNull(6) ? "" : reader.GetString(6)
                    });
                }
            }

            EmployeesGrid.ItemsSource = employees;
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            var emp = (EmployeeView)EmployeesGrid.SelectedItem;

            if (emp == null)
            {
                MessageBox.Show("Выберите сотрудника");
                return;
            }

            try
            {
                var db = new Data.Database();
                string details = "";

                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    // Информация о сотруднике
                    string userSql = @"
SELECT 
u.last_name,
u.first_name,
u.middle_name,
u.email,
u.phone,
u.department,
r.name
FROM users u
LEFT JOIN roles r ON u.role_id = r.id
WHERE u.id = @id";

                    var userCmd = new NpgsqlCommand(userSql, conn);
                    userCmd.Parameters.AddWithValue("id", emp.Id);

                    var reader = userCmd.ExecuteReader();

                    if (reader.Read())
                    {
                        details += $"👤 Сотрудник\n\n";
                        details += $"Фамилия: {reader.GetString(0)}\n";
                        details += $"Имя: {reader.GetString(1)}\n";

                        string middle = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        details += $"Отчество: {middle}\n";

                        details += $"Email: {reader.GetString(3)}\n";

                        string phone = reader.IsDBNull(4) ? "не указан" : reader.GetString(4);
                        details += $"Телефон: {phone}\n";

                        string dept = reader.IsDBNull(5) ? "не указан" : reader.GetString(5);
                        details += $"Отдел: {dept}\n";

                        string role = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        details += $"Роль: {role}\n\n";
                    }

                    reader.Close();

                    // История заявок
                    string ticketsSql = @"
SELECT 
t.id,
t.short_description,
s.name,
t.created_at
FROM tickets t
LEFT JOIN statuses s ON t.status_id = s.id
WHERE t.author_id = @id
   OR t.assignee_id = @id
ORDER BY t.created_at DESC";

                    var ticketCmd = new NpgsqlCommand(ticketsSql, conn);
                    ticketCmd.Parameters.AddWithValue("id", emp.Id);

                    var ticketReader = ticketCmd.ExecuteReader();

                    details += "📋 История заявок:\n\n";

                    if (!ticketReader.HasRows)
                    {
                        details += "Заявок нет.";
                    }
                    else
                    {
                        while (ticketReader.Read())
                        {
                            int id = ticketReader.GetInt32(0);
                            string desc = ticketReader.GetString(1);
                            string status = ticketReader.IsDBNull(2) ? "" : ticketReader.GetString(2);
                            DateTime date = ticketReader.GetDateTime(3);

                            details += $"#{id} | {desc}\n";
                            details += $"Статус: {status}\n";
                            details += $"Дата: {date:dd.MM.yyyy HH:mm}\n\n";
                        }
                    }
                }

                MessageBox.Show(details, "Информация о сотруднике");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }




        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

public class EmployeeView
{
    public int Id { get; set; }

    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public string Department { get; set; }

    public string Role { get; set; }
}