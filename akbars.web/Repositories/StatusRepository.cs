using System.Collections.Generic;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class StatusRepository : RepositoryBase, IStatusRepository
    {
        public StatusRepository(Database database) : base(database)
        {
        }

        public List<Status> GetStatuses()
        {
            var statuses = new List<Status>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name, description FROM statuses ORDER BY id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        statuses.Add(new Status
                        {
                            Id = reader.GetInt32(0),
                            Name = ReadNullableString(reader, 1),
                            Description = ReadNullableString(reader, 2)
                        });
                    }
                }
            }

            return statuses;
        }

        public void AddStatus(string name, string description)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO statuses (name, description) VALUES (@name, @description)", conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("description", (object)description ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
