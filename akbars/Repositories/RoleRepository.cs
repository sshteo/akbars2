using System.Collections.Generic;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class RoleRepository : RepositoryBase, IRoleRepository
    {
        public RoleRepository(Database database) : base(database)
        {
        }

        public List<Role> GetRoles()
        {
            var roles = new List<Role>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name, description FROM roles ORDER BY id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetInt32(0),
                            Name = ReadNullableString(reader, 1),
                            Description = ReadNullableString(reader, 2)
                        });
                    }
                }
            }

            return roles;
        }
    }
