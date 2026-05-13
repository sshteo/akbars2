using System.Collections.Generic;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class TicketTypeRepository : RepositoryBase, ITicketTypeRepository
    {
        public TicketTypeRepository(Database database) : base(database)
        {
        }

        public List<TicketType> GetTypes()
        {
            var types = new List<TicketType>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, name FROM ticket_types ORDER BY id", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(new TicketType
                        {
                            Id = reader.GetInt32(0),
                            Name = ReadNullableString(reader, 1)
                        });
                    }
                }
            }

            return types;
        }

        public void AddType(string name)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO ticket_types (name) VALUES (@name)", conn))
                {
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
