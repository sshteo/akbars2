using akbars.Models;

namespace akbars.ViewModels
{
    public class DispatcherDashboardViewModel
    {
        public required SessionContext Session { get; set; }

        public required TicketStatistics Stats { get; set; }

        public required List<User> Employees { get; set; }

        public required List<User> Executors { get; set; }
    }
}
