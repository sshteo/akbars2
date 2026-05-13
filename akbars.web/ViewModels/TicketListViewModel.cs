using akbars.Models;

namespace akbars.ViewModels
{
    public class TicketListViewModel
    {
        public required SessionContext Session { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;

        public string SearchText { get; set; } = string.Empty;

        public string StatusName { get; set; } = string.Empty;

        public string PriorityName { get; set; } = string.Empty;

        public bool AllowCreate { get; set; }

        public bool AllowDelete { get; set; }

        public bool AllowAssign { get; set; }

        public bool AllowStatusActions { get; set; }

        public required List<string> StatusOptions { get; set; }

        public required List<string> PriorityOptions { get; set; }

        public required List<TicketListItem> Tickets { get; set; }
    }
}
