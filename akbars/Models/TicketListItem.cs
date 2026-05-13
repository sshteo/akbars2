using System;

namespace akbars.Models
{
    public class TicketListItem
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string ShortDescription { get; set; }

        public string DetailedDescription { get; set; }

        public string TypeName { get; set; }

        public string PriorityName { get; set; }

        public string StatusName { get; set; }

        public string AuthorName { get; set; }

        public string AssigneeName { get; set; }
    }
}
