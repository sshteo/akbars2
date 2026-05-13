using System.Collections.Generic;

namespace akbars.Models
{
    public class TicketDetails : TicketListItem
    {
        public int PriorityId { get; set; }

        public int TypeId { get; set; }

        public int StatusId { get; set; }

        public int AuthorId { get; set; }

        public int AssigneeId { get; set; }

        public List<TicketHistory> History { get; set; }

        public TicketDetails()
        {
            History = new List<TicketHistory>();
        }
    }
}
