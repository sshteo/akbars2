using akbars.Models;

namespace akbars.ViewModels
{
    public class AdminDashboardViewModel
    {
        public required SessionContext Session { get; set; }

        public required TicketStatistics Stats { get; set; }

        public required List<User> Users { get; set; }

        public required List<Role> Roles { get; set; }

        public required List<Priority> Priorities { get; set; }

        public required List<Status> Statuses { get; set; }

        public required List<TicketType> TicketTypes { get; set; }
    }
}
