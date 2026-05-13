using akbars.Models;

namespace akbars.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        public required SessionContext Session { get; set; }

        public required TicketStatistics Stats { get; set; }
    }
}
