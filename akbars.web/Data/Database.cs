using Microsoft.Extensions.Configuration;
using Npgsql;

namespace akbars.Data
{
    public class Database
    {
        public Database(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("PostgresConnection") ?? string.Empty;
        }

        public string ConnectionString { get; }

        public bool HasConfiguredConnectionString()
        {
            return !string.IsNullOrWhiteSpace(ConnectionString);
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
    }
}
