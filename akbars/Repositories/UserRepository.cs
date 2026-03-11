using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class UserRepository
    {
        private readonly Database _database = new Database();

        public User GetUser(string login, string password)
        {
            using (var conn = _database.GetConnection())
            {
                conn.Open();

                string sql =
                @"SELECT id, first_name, last_name, middle_name, role_id
                  FROM users
                  WHERE login=@login AND password_hash=@password";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    cmd.Parameters.AddWithValue("password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                MiddleName = reader.GetString(3),
                                RoleId = reader.GetInt32(4)
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}