using akbars.Data;
using akbars.Models;
using Npgsql;
using System.Collections.Generic;

namespace akbars.Repositories
{
    public class RoleRepository
    {
        private readonly Database _database = new Database();

        public List<Role> GetRoles()
        {
            var roles = new List<Role>();

            using (var connection = _database.GetConnection())
            {
                connection.Open();

                string sql = "SELECT id, name, description FROM roles";

                using (var cmd = new NpgsqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        });
                    }
                }
            }

            return roles;
        }
    }
}