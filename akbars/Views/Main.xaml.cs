using akbars.Models;
using akbars.Views.Admin;
using akbars.Views.Dispatcher;
using akbars.Views.Repairman;
using akbars.Views.Worker;
using Npgsql;
using System.Windows;

namespace akbars.Views
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            if (Properties.Settings.Default.RememberMe)
            {
                LoginBox.Text = Properties.Settings.Default.SavedLogin;
                PasswordBox.Password = Properties.Settings.Default.SavedPassword;
                RememberMeCheck.IsChecked = true;
            }

        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;
            string password = PasswordBox.Password;

            var db = new Data.Database();

            using (var conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT 
                                first_name,
                                last_name,
                                middle_name,
                                email,
                                phone,
                                department,
                                role_id
                               FROM users
                               WHERE login=@login AND password_hash=@password";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string firstName = reader.GetString(0);
                        string lastName = reader.GetString(1);
                        string middleName = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        string email = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        string phone = reader.IsDBNull(4) ? "" : reader.GetString(4);
                        string department = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        int role = reader.GetInt32(6);

                        string roleName = "";

                        switch (role)
                        {
                            case 1:
                                roleName = "Сотрудник";
                                new MainWorker(
                                    lastName,
                                    firstName,
                                    middleName,
                                    email,
                                    phone,
                                    department,
                                    roleName
                                ).Show();
                                this.Close();
                                break;

                            case 2:
                                roleName = "Исполнитель";
                              /*  new MainRepairman(
                                    lastName,
                                    firstName,
                                    middleName,
                                    email,
                                    phone,
                                    department,
                                    roleName
                              */
                            //    ).
                                Show();
                                this.Close();
                                break;

                            case 3:
                               new MainDispatcher(
                                      lastName,
                                      firstName,
                                      middleName,
                                      email,
                                      phone,
                                      department,
                                      roleName
                                      
                                  ).
                                Show();
                                this.Close();
                                break;

                            case 4:
                                roleName = "Администратор";
                                /*  
                                 new MainAdmin(
                                     lastName,
                                     firstName,
                                     middleName,
                                     email,
                                     phone,
                                     department,
                                     roleName
                                     */
                                //    ).
                                Show();
                                this.Close();
                                break;

                            default:
                                MessageBox.Show("Неизвестная роль");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                    }
                }
            }
            if (RememberMeCheck.IsChecked == true)
            {
                Properties.Settings.Default.SavedLogin = login;
                Properties.Settings.Default.SavedPassword = password;
                Properties.Settings.Default.RememberMe = true;
            }
            else
            {
                Properties.Settings.Default.SavedLogin = "";
                Properties.Settings.Default.SavedPassword = "";
                Properties.Settings.Default.RememberMe = false;
            }

            Properties.Settings.Default.Save();
        }
    }
}