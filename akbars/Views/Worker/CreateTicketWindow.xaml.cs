using Npgsql;
using System.Collections.Generic;
using System.Windows;

namespace akbars.Views.Worker
{
    public partial class CreateTicketWindow : Window
    {
        private int currentUserId;

        private Dictionary<string, int> priorityMap = new Dictionary<string, int>();
        private Dictionary<string, int> typeMap = new Dictionary<string, int>();

        public CreateTicketWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;

            LoadData();
        }

        private void LoadData()
        {
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                // типы неисправностей
                var typeCmd = new NpgsqlCommand("SELECT id, name FROM ticket_types", conn);
                var typeReader = typeCmd.ExecuteReader();

                while (typeReader.Read())
                {
                    int id = typeReader.GetInt32(0);
                    string name = typeReader.GetString(1);

                    typeMap[name] = id;
                    TypeBox.Items.Add(name);
                }

                typeReader.Close();

                // приоритеты
                var prCmd = new NpgsqlCommand("SELECT id, name FROM priorities", conn);
                var prReader = prCmd.ExecuteReader();

                while (prReader.Read())
                {
                    int id = prReader.GetInt32(0);
                    string name = prReader.GetString(1);

                    priorityMap[name] = id;
                    PriorityBox.Items.Add(name);
                }
            }

            TypeBox.SelectedIndex = 0;
            PriorityBox.SelectedIndex = 1;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string problem = ProblemBox.Text;

            if (string.IsNullOrWhiteSpace(problem))
            {
                MessageBox.Show("Введите описание проблемы");
                return;
            }

            string typeName = TypeBox.SelectedItem.ToString();
            string priorityName = PriorityBox.SelectedItem.ToString();

            int typeId = typeMap[typeName];
            int priorityId = priorityMap[priorityName];

            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"
                INSERT INTO tickets
                (author_id, short_description, type_id, priority_id, status_id)
                VALUES
                (@author, @desc, @type, @priority, 1)";

                var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("author", currentUserId);
                cmd.Parameters.AddWithValue("desc", problem);
                cmd.Parameters.AddWithValue("type", typeId);
                cmd.Parameters.AddWithValue("priority", priorityId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Заявка создана");

            this.Close();
        }
    }
}