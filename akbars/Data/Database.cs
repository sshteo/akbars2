using Npgsql;
using System.Configuration;

namespace akbars.Data
{
    public class Database
    {
        public string ConnectionString
        {
            get
            {
                var settings = ConfigurationManager.ConnectionStrings["PostgresConnection"];
                return settings == null ? string.Empty : settings.ConnectionString;
            }
        }

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
