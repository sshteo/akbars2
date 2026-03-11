using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using akbars.Data;
using akbars.Models;
using Npgsql;
using System.Collections.Generic;

namespace akbars.Repositories
{
    public class TicketRepository
    {
        private readonly Database _database = new Database();

        public List<Ticket> GetTickets()
        {
            var list = new List<Ticket>();

            using (var conn = _database.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT id, created_at, updated_at,
                               short_description, detailed_description,
                               priority_id, type_id, status_id,
                               author_id, assignee_id
                               FROM tickets";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Ticket
                        {
                            Id = reader.GetInt32(0),
                            CreatedAt = reader.GetDateTime(1),
                            UpdatedAt = reader.GetDateTime(2),
                            ShortDescription = reader.GetString(3),
                            DetailedDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                            PriorityId = reader.GetInt32(5),
                            TypeId = reader.GetInt32(6),
                            StatusId = reader.GetInt32(7),
                            AuthorId = reader.GetInt32(8),
                            AssigneeId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                        });
                    }
                }
            }

            return list;
        }
    }
}