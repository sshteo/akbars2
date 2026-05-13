using akbars.Models;

namespace akbars.ViewModels
{
    public class TicketDetailsViewModel
    {
        public required SessionContext Session { get; set; }

        public required TicketDetails Ticket { get; set; }

        public bool AllowStatusActions { get; set; }

        public string ActionNote { get; set; } = string.Empty;
    }
}
