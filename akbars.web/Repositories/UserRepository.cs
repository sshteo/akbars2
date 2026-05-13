using System.Collections.Generic;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(Database database) : base(database)
        {
        }

        public User GetByLogin(string login)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT u.id, u.last_name, u.first_name, u.middle_name, u.email, u.phone,
                           u.login, u.password_hash, u.role_id, u.department, r.name
                    FROM users u
                    LEFT JOIN roles r ON r.id = u.role_id
                    WHERE u.login = @login", conn))
                {
                    cmd.Parameters.AddWithValue("login", login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? MapUser(reader) : null;
                    }
                }
            }
        }

        public User GetById(int userId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    SELECT u.id, u.last_name, u.first_name, u.middle_name, u.email, u.phone,
                           u.login, u.password_hash, u.role_id, u.department, r.name
                    FROM users u
                    LEFT JOIN roles r ON r.id = u.role_id
                    WHERE u.id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? MapUser(reader) : null;
                    }
                }
            }
        }

        public List<User> GetUsers(int? roleId)
        {
            var users = new List<User>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var sql = @"
                    SELECT u.id, u.last_name, u.first_name, u.middle_name, u.email, u.phone,
                           u.login, u.password_hash, u.role_id, u.department, r.name
                    FROM users u
                    LEFT JOIN roles r ON r.id = u.role_id
                    WHERE 1 = 1";

                if (roleId.HasValue)
                {
                    sql += " AND u.role_id = @roleId";
                }

                sql += " ORDER BY u.last_name, u.first_name";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (roleId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("roleId", roleId.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(MapUser(reader));
                        }
                    }
                }
            }

            return users;
        }

        public void UpdateProfile(User user)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
                    UPDATE users
                    SET last_name = @lastName,
                        first_name = @firstName,
                        middle_name = @middleName,
                        email = @email,
                        phone = @phone,
                        department = @department
                    WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", user.Id);
                    cmd.Parameters.AddWithValue("lastName", user.LastName ?? string.Empty);
                    cmd.Parameters.AddWithValue("firstName", user.FirstName ?? string.Empty);
                    cmd.Parameters.AddWithValue("middleName", (object)user.MiddleName ?? string.Empty);
                    cmd.Parameters.AddWithValue("email", (object)user.Email ?? string.Empty);
                    cmd.Parameters.AddWithValue("phone", (object)user.Phone ?? string.Empty);
                    cmd.Parameters.AddWithValue("department", (object)user.Department ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRole(int userId, int roleId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET role_id = @roleId WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("roleId", roleId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePasswordHash(int userId, string passwordHash)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE users SET password_hash = @passwordHash WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", userId);
                    cmd.Parameters.AddWithValue("passwordHash", passwordHash);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(0),
                LastName = ReadNullableString(reader, 1),
                FirstName = ReadNullableString(reader, 2),
                MiddleName = ReadNullableString(reader, 3),
                Email = ReadNullableString(reader, 4),
                Phone = ReadNullableString(reader, 5),
                Login = ReadNullableString(reader, 6),
                PasswordHash = ReadNullableString(reader, 7),
                RoleId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                Department = ReadNullableString(reader, 9),
                RoleName = ReadNullableString(reader, 10)
            };
        }
    }
}
