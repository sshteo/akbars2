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
    public class PriorityRepository
    {
        private readonly Database _database = new Database();

        public List<Priority> GetPriorities()
        {
            var list = new List<Priority>();

            using (var conn = _database.GetConnection())
            {
                conn.Open();

                string sql = "SELECT id, name, sla_hours FROM priorities";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Priority
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            SlaHours = reader.GetInt32(2)
                        });
                    }
                }
            }

            return list;
        }
    }
}
