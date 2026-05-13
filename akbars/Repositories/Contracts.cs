using System.Collections.Generic;
using akbars.Models;

namespace akbars.Repositories
{
    public interface IUserRepository
    {
        User GetByLogin(string login);

        User GetById(int userId);

        List<User> GetUsers(int? roleId);

        void UpdateProfile(User user);

        void UpdateRole(int userId, int roleId);

        void UpdatePasswordHash(int userId, string passwordHash);
    }

    public interface ITicketRepository
    {
        List<TicketListItem> GetTickets(TicketQuery query);

        TicketDetails GetTicketDetails(int ticketId);

        TicketStatistics GetStatistics(int? authorId, int? assigneeId);

        int CreateTicket(Ticket ticket);

        void AssignTicket(int ticketId, int executorId, int changedByUserId);

        void UpdateTicketStatus(int ticketId, int statusId, int changedByUserId, string note);

        bool DeleteTicket(int ticketId, int authorId);
    }

    public interface IPriorityRepository
    {
        List<Priority> GetPriorities();

        void AddPriority(string name, int slaHours);
    }

    public interface IRoleRepository
    {
        List<Role> GetRoles();
    }

    public interface IStatusRepository
    {
        List<Status> GetStatuses();

        void AddStatus(string name, string description);
    }

    public interface ITicketTypeRepository
    {
        List<TicketType> GetTypes();

        void AddType(string name);
    }
}
