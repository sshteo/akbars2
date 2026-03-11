using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Npgsql;


namespace akbars.Data
{
    public class Database
    {
        private string _connectionString =
            "Host=localhost;Port=5432;Database=akbars;Username=postgres;Password=2240630";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
