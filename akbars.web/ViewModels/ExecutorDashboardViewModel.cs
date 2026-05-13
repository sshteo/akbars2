using akbars.Models;

namespace akbars.ViewModels
{
    public class ExecutorDashboardViewModel
    {
        public required SessionContext Session { get; set; }

        public required TicketStatistics Stats { get; set; }

        public required List<TicketListItem> Tickets { get; set; }
    }
}
