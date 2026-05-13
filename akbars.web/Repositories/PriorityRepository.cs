using System.Collections.Generic;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class PriorityRepository : RepositoryBase, IPriorityRepository
    {
        public PriorityRepository(Database database) : base(database)
        {
        }

        public List<Priority> GetPriorities()
        {
            var priorities = new List<Priority>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name, sla_hours FROM priorities ORDER BY id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        priorities.Add(new Priority
                        {
                            Id = reader.GetInt32(0),
                            Name = ReadNullableString(reader, 1),
                            SlaHours = reader.IsDBNull(2) ? 0 : reader.GetInt32(2)
                        });
                    }
                }
            }

            return priorities;
        }

        public void AddPriority(string name, int slaHours)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO priorities (name, sla_hours) VALUES (@name, @slaHours)", conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("slaHours", slaHours);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
