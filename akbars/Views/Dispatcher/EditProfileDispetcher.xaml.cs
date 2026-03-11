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
    public partial class EditProfileDispetcher : Window
    {
        private int userId;

        public EditProfileDispetcher(int id)
        {
            InitializeComponent();
            userId = id;

            LoadUser();
        }

        private void LoadUser()
        {
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT last_name, first_name, middle_name, email, phone 
                               FROM users
                               WHERE id = @id";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("id", userId);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    LastNameBox.Text = reader.GetString(0);
                    FirstNameBox.Text = reader.GetString(1);
                    MiddleNameBox.Text = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    EmailBox.Text = reader.GetString(3);
                    PhoneBox.Text = reader.IsDBNull(4) ? "" : reader.GetString(4);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"UPDATE users
                               SET last_name=@ln,
                                   first_name=@fn,
                                   middle_name=@mn,
                                   email=@em,
                                   phone=@ph
                               WHERE id=@id";

                var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("ln", LastNameBox.Text);
                cmd.Parameters.AddWithValue("fn", FirstNameBox.Text);
                cmd.Parameters.AddWithValue("mn", MiddleNameBox.Text);
                cmd.Parameters.AddWithValue("em", EmailBox.Text);
                cmd.Parameters.AddWithValue("ph", PhoneBox.Text);
                cmd.Parameters.AddWithValue("id", userId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Данные успешно обновлены");

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}