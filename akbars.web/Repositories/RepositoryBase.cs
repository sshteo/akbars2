using System;
using System.Collections.Generic;
using System.Globalization;
using akbars.Data;
using Npgsql;

namespace akbars.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly Dictionary<string, bool> _schemaCache = new Dictionary<string, bool>();

        protected RepositoryBase(Database database)
        {
            Database = database;
        }

        protected Database Database { get; private set; }

        protected bool HasTable(NpgsqlConnection connection, string tableName)
        {
            return HasSchemaObject(connection, "table", tableName, null);
        }

        protected bool HasColumn(NpgsqlConnection connection, string tableName, string columnName)
        {
            return HasSchemaObject(connection, "column", tableName, columnName);
        }

        protected string GetAssigneeColumnName(NpgsqlConnection connection)
        {
            if (HasColumn(connection, "tickets", "assignee_id"))
            {
                return "assignee_id";
            }

            if (HasColumn(connection, "tickets", "executor_id"))
            {
                return "executor_id";
            }

            return null;
        }

        protected bool HasCompletedAtColumn(NpgsqlConnection connection)
        {
            return HasColumn(connection, "tickets", "completed_at");
        }

        protected void WriteHistoryIfPossible(
            NpgsqlConnection connection,
            int ticketId,
            int changedByUserId,
            string fieldName,
            string oldValue,
            string newValue)
        {
            if (HasTable(connection, "ticket_history"))
            {
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO ticket_history (ticket_id, changed_at, changed_by, field_name, old_value, new_value)
                      VALUES (@ticket, NOW(), @changedBy, @field, @oldValue, @newValue)", connection))
                {
                    cmd.Parameters.AddWithValue("ticket", ticketId);
                    cmd.Parameters.AddWithValue("changedBy", changedByUserId);
                    cmd.Parameters.AddWithValue("field", fieldName);
                    cmd.Parameters.AddWithValue("oldValue", (object)oldValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("newValue", (object)newValue ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            else if (HasTable(connection, "ticket_histories"))
            {
                using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO ticket_histories (ticket_id, changed_at, changed_by, field_name, old_value, new_value)
                      VALUES (@ticket, NOW(), @changedBy, @field, @oldValue, @newValue)", connection))
                {
                    cmd.Parameters.AddWithValue("ticket", ticketId);
                    cmd.Parameters.AddWithValue("changedBy", changedByUserId);
                    cmd.Parameters.AddWithValue("field", fieldName);
                    cmd.Parameters.AddWithValue("oldValue", (object)oldValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("newValue", (object)newValue ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected string ReadNullableString(NpgsqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        protected DateTime? ReadNullableDateTime(NpgsqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? (DateTime?)null : reader.GetDateTime(ordinal);
        }

        protected string NormalizeStatusName(string statusName)
        {
            var normalized = (statusName ?? string.Empty).Trim().ToLowerInvariant();

            if (normalized.Contains("нов"))
            {
                return "new";
            }

            if (normalized.Contains("работ") || normalized.Contains("progress") || normalized.Contains("назнач"))
            {
                return "in_progress";
            }

            if (normalized.Contains("выполн") || normalized.Contains("закры") || normalized.Contains("complete"))
            {
                return "completed";
            }

            if (normalized.Contains("отмен") || normalized.Contains("отклон") || normalized.Contains("cancel"))
            {
                return "cancelled";
            }

            return normalized;
        }

        protected int ResolveStatusId(NpgsqlConnection connection, string normalizedStatus)
        {
            using (var cmd = new NpgsqlCommand("SELECT id, name FROM statuses ORDER BY id", connection))
            using (var reader = cmd.ExecuteReader())
            {
                var fallback = 0;

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var name = reader.GetString(1);
                    if (fallback == 0)
                    {
                        fallback = id;
                    }

                    if (NormalizeStatusName(name) == normalizedStatus)
                    {
                        return id;
                    }
                }

                return fallback;
            }
        }

        private bool HasSchemaObject(NpgsqlConnection connection, string kind, string tableName, string columnName)
        {
            var key = string.Format(
                CultureInfo.InvariantCulture,
                "{0}:{1}:{2}",
                kind,
                tableName ?? string.Empty,
                columnName ?? string.Empty);

            bool exists;
            if (_schemaCache.TryGetValue(key, out exists))
            {
                return exists;
            }

            string sql;
            if (kind == "table")
            {
                sql = @"SELECT COUNT(*)
                        FROM information_schema.tables
                        WHERE table_schema = 'public' AND table_name = @table";
            }
            else
            {
                sql = @"SELECT COUNT(*)
                        FROM information_schema.columns
                        WHERE table_schema = 'public' AND table_name = @table AND column_name = @column";
            }

            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("table", tableName);
                if (kind == "column")
                {
                    cmd.Parameters.AddWithValue("column", columnName);
                }

                exists = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
                _schemaCache[key] = exists;
                return exists;
            }
        }
    }
}
