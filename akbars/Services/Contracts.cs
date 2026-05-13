using System.Collections.Generic;
using akbars.Models;

namespace akbars.Services
{
    public interface IAuthService
    {
        bool CanConnect(out string errorMessage);

        AuthResult Authenticate(string login, string password);
    }

    public interface ITicketService
    {
        TicketStatistics GetStatistics(int? authorId, int? assigneeId);

        List<TicketListItem> GetTickets(TicketQuery query);

        TicketDetails GetTicketDetails(int ticketId);

        int CreateTicket(int authorId, string shortDescription, string detailedDescription, int priorityId, int typeId);

        void AssignTicket(int ticketId, int executorId, int changedByUserId);

        void StartTicket(int ticketId, int changedByUserId, string note);

        void CompleteTicket(int ticketId, int changedByUserId, string note);

        void CancelTicket(int ticketId, int changedByUserId, string note);

        bool DeleteTicket(int ticketId, int authorId);
    }

    public interface IUserService
    {
        User GetUser(int userId);

        void UpdateProfile(User user);

        List<User> GetUsers(int? roleId);

        void UpdateRole(int userId, int roleId);
    }

    public interface ILookupService
    {
        List<Priority> GetPriorities();

        List<Status> GetStatuses();

        List<TicketType> GetTicketTypes();

        List<Role> GetRoles();

        void AddPriority(string name, int slaHours);

        void AddStatus(string name, string description);

        void AddTicketType(string name);
    }
}
