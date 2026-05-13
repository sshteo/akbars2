using System;
using System.Collections.Generic;
using System.Text;
using akbars.Data;
using akbars.Models;
using Npgsql;

namespace akbars.Repositories
{
    public class TicketRepository : RepositoryBase, ITicketRepository
    {
        public TicketRepository(Database database) : base(database)
        {
        }

        public List<TicketListItem> GetTickets(TicketQuery query)
        {
            var tickets = new List<TicketListItem>();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var assigneeColumn = GetAssigneeColumnName(conn);
                var hasCompletedAt = HasCompletedAtColumn(conn);

                var sql = new StringBuilder();
                sql.AppendLine("SELECT t.id, t.created_at, t.updated_at,");
                sql.AppendLine("       t.short_description, t.detailed_description,");
                sql.AppendLine("       tt.name AS type_name, p.name AS priority_name, s.name AS status_name,");
                sql.AppendLine("       author.first_name || ' ' || author.last_name AS author_name,");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("       assignee.first_name || ' ' || assignee.last_name AS assignee_name,");
                }
                else
                {
                    sql.AppendLine("       NULL AS assignee_name,");
                }
                sql.AppendLine(hasCompletedAt ? "       t.completed_at" : "       NULL AS completed_at");
                sql.AppendLine("FROM tickets t");
                sql.AppendLine("LEFT JOIN ticket_types tt ON tt.id = t.type_id");
                sql.AppendLine("LEFT JOIN priorities p ON p.id = t.priority_id");
                sql.AppendLine("LEFT JOIN statuses s ON s.id = t.status_id");
                sql.AppendLine("LEFT JOIN users author ON author.id = t.author_id");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("LEFT JOIN users assignee ON assignee.id = t." + assigneeColumn);
                }
                sql.AppendLine("WHERE 1 = 1");

                if (query.AuthorId.HasValue)
                {
                    sql.AppendLine("AND t.author_id = @authorId");
                }

                if (query.AssigneeId.HasValue && !string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("AND t." + assigneeColumn + " = @assigneeId");
                }

                if (!string.IsNullOrWhiteSpace(query.StatusName))
                {
                    sql.AppendLine("AND s.name = @statusName");
                }

                if (!string.IsNullOrWhiteSpace(query.PriorityName))
                {
                    sql.AppendLine("AND p.name = @priorityName");
                }

                if (!string.IsNullOrWhiteSpace(query.SearchText))
                {
                    sql.AppendLine("AND (LOWER(t.short_description) LIKE @search OR LOWER(COALESCE(t.detailed_description, '')) LIKE @search)");
                }

                sql.AppendLine("ORDER BY t.updated_at DESC, t.id DESC");

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    if (query.AuthorId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("authorId", query.AuthorId.Value);
                    }

                    if (query.AssigneeId.HasValue && !string.IsNullOrWhiteSpace(assigneeColumn))
                    {
                        cmd.Parameters.AddWithValue("assigneeId", query.AssigneeId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(query.StatusName))
                    {
                        cmd.Parameters.AddWithValue("statusName", query.StatusName);
                    }

                    if (!string.IsNullOrWhiteSpace(query.PriorityName))
                    {
                        cmd.Parameters.AddWithValue("priorityName", query.PriorityName);
                    }

                    if (!string.IsNullOrWhiteSpace(query.SearchText))
                    {
                        cmd.Parameters.AddWithValue("search", "%" + query.SearchText.Trim().ToLowerInvariant() + "%");
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tickets.Add(new TicketListItem
                            {
                                Id = reader.GetInt32(0),
                                CreatedAt = reader.GetDateTime(1),
                                UpdatedAt = reader.GetDateTime(2),
                                ShortDescription = ReadNullableString(reader, 3),
                                DetailedDescription = ReadNullableString(reader, 4),
                                TypeName = ReadNullableString(reader, 5),
                                PriorityName = ReadNullableString(reader, 6),
                                StatusName = ReadNullableString(reader, 7),
                                AuthorName = ReadNullableString(reader, 8),
                                AssigneeName = ReadNullableString(reader, 9),
                                CompletedAt = ReadNullableDateTime(reader, 10)
                            });
                        }
                    }
                }
            }

            return tickets;
        }

        public TicketDetails GetTicketDetails(int ticketId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var assigneeColumn = GetAssigneeColumnName(conn);
                var hasCompletedAt = HasCompletedAtColumn(conn);

                var sql = new StringBuilder();
                sql.AppendLine("SELECT t.id, t.created_at, t.updated_at,");
                sql.AppendLine("       t.short_description, t.detailed_description,");
                sql.AppendLine("       t.priority_id, t.type_id, t.status_id, t.author_id,");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("       COALESCE(t." + assigneeColumn + ", 0),");
                }
                else
                {
                    sql.AppendLine("       0,");
                }
                sql.AppendLine("       p.name, tt.name, s.name,");
                sql.AppendLine("       author.first_name || ' ' || author.last_name,");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("       assignee.first_name || ' ' || assignee.last_name,");
                }
                else
                {
                    sql.AppendLine("       NULL,");
                }
                sql.AppendLine(hasCompletedAt ? "       t.completed_at" : "       NULL");
                sql.AppendLine("FROM tickets t");
                sql.AppendLine("LEFT JOIN priorities p ON p.id = t.priority_id");
                sql.AppendLine("LEFT JOIN ticket_types tt ON tt.id = t.type_id");
                sql.AppendLine("LEFT JOIN statuses s ON s.id = t.status_id");
                sql.AppendLine("LEFT JOIN users author ON author.id = t.author_id");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("LEFT JOIN users assignee ON assignee.id = t." + assigneeColumn);
                }
                sql.AppendLine("WHERE t.id = @id");

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("id", ticketId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        var details = new TicketDetails
                        {
                            Id = reader.GetInt32(0),
                            CreatedAt = reader.GetDateTime(1),
                            UpdatedAt = reader.GetDateTime(2),
                            ShortDescription = ReadNullableString(reader, 3),
                            DetailedDescription = ReadNullableString(reader, 4),
                            PriorityId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                            TypeId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                            StatusId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                            AuthorId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                            AssigneeId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                            PriorityName = ReadNullableString(reader, 10),
                            TypeName = ReadNullableString(reader, 11),
                            StatusName = ReadNullableString(reader, 12),
                            AuthorName = ReadNullableString(reader, 13),
                            AssigneeName = ReadNullableString(reader, 14),
                            CompletedAt = ReadNullableDateTime(reader, 15)
                        };

                        reader.Close();
                        details.History = LoadHistory(conn, ticketId);
                        return details;
                    }
                }
            }
        }

        public TicketStatistics GetStatistics(int? authorId, int? assigneeId)
        {
            var stats = new TicketStatistics();

            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var assigneeColumn = GetAssigneeColumnName(conn);

                var sql = new StringBuilder();
                sql.AppendLine("SELECT s.name, COUNT(*),");
                sql.AppendLine("       COUNT(CASE");
                sql.AppendLine("           WHEN s.name NOT ILIKE '%выполн%' AND s.name NOT ILIKE '%отмен%'");
                sql.AppendLine("             AND s.name NOT ILIKE '%отклон%'");
                sql.AppendLine("             AND p.sla_hours IS NOT NULL");
                sql.AppendLine("             AND t.created_at + make_interval(hours => p.sla_hours) < NOW()");
                sql.AppendLine("           THEN 1 END)");
                sql.AppendLine("FROM tickets t");
                sql.AppendLine("LEFT JOIN statuses s ON s.id = t.status_id");
                sql.AppendLine("LEFT JOIN priorities p ON p.id = t.priority_id");
                sql.AppendLine("WHERE 1 = 1");

                if (authorId.HasValue)
                {
                    sql.AppendLine("AND t.author_id = @authorId");
                }

                if (assigneeId.HasValue && !string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("AND t." + assigneeColumn + " = @assigneeId");
                }

                sql.AppendLine("GROUP BY s.name");

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    if (authorId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("authorId", authorId.Value);
                    }

                    if (assigneeId.HasValue && !string.IsNullOrWhiteSpace(assigneeColumn))
                    {
                        cmd.Parameters.AddWithValue("assigneeId", assigneeId.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var statusName = ReadNullableString(reader, 0);
                            var count = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            var overdue = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);

                            stats.Total += count;
                            stats.OverdueCount += overdue;

                            switch (NormalizeStatusName(statusName))
                            {
                                case "new":
                                    stats.NewCount += count;
                                    break;
                                case "in_progress":
                                    stats.InProgressCount += count;
                                    break;
                                case "completed":
                                    stats.CompletedCount += count;
                                    break;
                                case "cancelled":
                                    stats.CancelledCount += count;
                                    break;
                            }
                        }
                    }
                }
            }

            return stats;
        }

        public int CreateTicket(Ticket ticket)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var statusId = ResolveStatusId(conn, "new");
                using (var cmd = new NpgsqlCommand(@"
                    INSERT INTO tickets
                    (created_at, updated_at, short_description, detailed_description, priority_id, type_id, status_id, author_id)
                    VALUES (NOW(), NOW(), @shortDescription, @detailedDescription, @priorityId, @typeId, @statusId, @authorId)
                    RETURNING id", conn))
                {
                    cmd.Parameters.AddWithValue("shortDescription", ticket.ShortDescription);
                    cmd.Parameters.AddWithValue("detailedDescription", (object)ticket.DetailedDescription ?? string.Empty);
                    cmd.Parameters.AddWithValue("priorityId", ticket.PriorityId);
                    cmd.Parameters.AddWithValue("typeId", ticket.TypeId);
                    cmd.Parameters.AddWithValue("statusId", statusId);
                    cmd.Parameters.AddWithValue("authorId", ticket.AuthorId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void AssignTicket(int ticketId, int executorId, int changedByUserId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var assigneeColumn = GetAssigneeColumnName(conn);
                if (string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    throw new InvalidOperationException("В таблице tickets отсутствует колонка assignee_id/executor_id.");
                }

                var previous = GetTicketDetails(ticketId);
                var inProgressStatusId = ResolveStatusId(conn, "in_progress");

                var sql = new StringBuilder();
                sql.AppendLine("UPDATE tickets");
                sql.AppendLine("SET " + assigneeColumn + " = @executorId, status_id = @statusId, updated_at = NOW()");
                if (HasCompletedAtColumn(conn))
                {
                    sql.AppendLine(", completed_at = NULL");
                }
                sql.AppendLine("WHERE id = @ticketId");

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("executorId", executorId);
                    cmd.Parameters.AddWithValue("statusId", inProgressStatusId);
                    cmd.Parameters.AddWithValue("ticketId", ticketId);
                    cmd.ExecuteNonQuery();
                }

                WriteHistoryIfPossible(conn, ticketId, changedByUserId, "executor", previous == null ? null : previous.AssigneeName, executorId.ToString());
                WriteHistoryIfPossible(conn, ticketId, changedByUserId, "status", previous == null ? null : previous.StatusName, "В работе");
            }
        }

        public void UpdateTicketStatus(int ticketId, int statusId, int changedByUserId, string note)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var previous = GetTicketDetails(ticketId);
                string normalizedStatus;
                switch (statusId)
                {
                    case 2:
                        normalizedStatus = "in_progress";
                        break;
                    case 3:
                        normalizedStatus = "completed";
                        break;
                    case 4:
                        normalizedStatus = "cancelled";
                        break;
                    default:
                        normalizedStatus = "in_progress";
                        break;
                }

                var resolvedStatusId = ResolveStatusId(conn, normalizedStatus);
                var sql = new StringBuilder();
                sql.AppendLine("UPDATE tickets");
                sql.AppendLine("SET status_id = @statusId, updated_at = NOW()");
                if (HasCompletedAtColumn(conn))
                {
                    if (normalizedStatus == "completed")
                    {
                        sql.AppendLine(", completed_at = NOW()");
                    }
                    else if (normalizedStatus == "in_progress" || normalizedStatus == "cancelled")
                    {
                        sql.AppendLine(", completed_at = NULL");
                    }
                }
                sql.AppendLine("WHERE id = @ticketId");

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("statusId", resolvedStatusId);
                    cmd.Parameters.AddWithValue("ticketId", ticketId);
                    cmd.ExecuteNonQuery();
                }

                var newStatusName = normalizedStatus == "in_progress"
                    ? "В работе"
                    : normalizedStatus == "completed"
                        ? "Выполнена"
                        : normalizedStatus == "cancelled"
                            ? "Отменена"
                            : statusId.ToString();
                WriteHistoryIfPossible(conn, ticketId, changedByUserId, "status", previous == null ? null : previous.StatusName, newStatusName);

                if (!string.IsNullOrWhiteSpace(note))
                {
                    WriteHistoryIfPossible(conn, ticketId, changedByUserId, "note", null, note.Trim());
                }
            }
        }

        public bool DeleteTicket(int ticketId, int authorId)
        {
            using (var conn = Database.GetConnection())
            {
                conn.Open();
                var assigneeColumn = GetAssigneeColumnName(conn);
                var sql = new StringBuilder();
                sql.AppendLine("DELETE FROM tickets");
                sql.AppendLine("WHERE id = @ticketId AND author_id = @authorId");
                if (!string.IsNullOrWhiteSpace(assigneeColumn))
                {
                    sql.AppendLine("AND " + assigneeColumn + " IS NULL");
                }

                using (var cmd = new NpgsqlCommand(sql.ToString(), conn))
                {
                    cmd.Parameters.AddWithValue("ticketId", ticketId);
                    cmd.Parameters.AddWithValue("authorId", authorId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private List<TicketHistory> LoadHistory(NpgsqlConnection conn, int ticketId)
        {
            var history = new List<TicketHistory>();
            string tableName = null;

            if (HasTable(conn, "ticket_history"))
            {
                tableName = "ticket_history";
            }
            else if (HasTable(conn, "ticket_histories"))
            {
                tableName = "ticket_histories";
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                return history;
            }

            using (var cmd = new NpgsqlCommand(
                "SELECT id, ticket_id, changed_at, changed_by, field_name, old_value, new_value FROM " + tableName + " WHERE ticket_id = @ticketId ORDER BY changed_at DESC", conn))
            {
                cmd.Parameters.AddWithValue("ticketId", ticketId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        history.Add(new TicketHistory
                        {
                            Id = reader.GetInt32(0),
                            TicketId = reader.GetInt32(1),
                            ChangedAt = reader.GetDateTime(2),
                            ChangedBy = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                            FieldName = ReadNullableString(reader, 4),
                            OldValue = ReadNullableString(reader, 5),
                            NewValue = ReadNullableString(reader, 6)
                        });
                    }
                }
            }

            return history;
        }
    }
}
