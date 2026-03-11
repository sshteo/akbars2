using akbars.Views.Dispatcher;
using Npgsql;
using Npgsql;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace akbars.Views.Dispatcher
{
    /// <summary>
    /// Логика взаимодействия для WorkersDispetcher.xaml
    /// </summary>
    public partial class WorkersDispetcher : Window
    {
        public WorkersDispetcher()
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
WHERE u.role_id = 3";

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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}