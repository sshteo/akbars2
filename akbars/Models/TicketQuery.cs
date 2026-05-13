namespace akbars.Models
{
    public class TicketQuery
    {
        public int? AuthorId { get; set; }

        public int? AssigneeId { get; set; }

        public string StatusName { get; set; }

        public string PriorityName { get; set; }

        public string SearchText { get; set; }
    }
}
