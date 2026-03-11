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
    public class TicketTypeRepository
    {
        private readonly Database _database = new Database();

        public List<TicketType> GetTypes()
        {
            var list = new List<TicketType>();

            using (var conn = _database.GetConnection())
            {
                conn.Open();

                string sql = "SELECT id, name FROM ticket_types";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TicketType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return list;
        }
    }
}