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
    public class StatusRepository
    {
        private readonly Database _database = new Database();

        public List<Status> GetStatuses()
        {
            var list = new List<Status>();

            using (var conn = _database.GetConnection())
            {
                conn.Open();

                string sql = "SELECT id, name, description FROM statuses";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Status
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        });
                    }
                }
            }

            return list;
        }
    }
}