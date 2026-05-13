using akbars.Data;
using akbars.Models;
using akbars.Repositories;

namespace akbars.Services
{
    public static class AppServices
    {
        public static AppSettingsService Settings { get; private set; }

        public static IAuthService AuthService { get; private set; }

        public static ITicketService TicketService { get; private set; }

        public static IUserService UserService { get; private set; }

        public static ILookupService LookupService { get; private set; }

        public static SessionContext CurrentSession { get; set; }

        public static void Initialize()
        {
            var database = new Database();
            var passwordHasher = new PasswordHasher();

            var userRepository = new UserRepository(database);
            var ticketRepository = new TicketRepository(database);
            var priorityRepository = new PriorityRepository(database);
            var statusRepository = new StatusRepository(database);
            var ticketTypeRepository = new TicketTypeRepository(database);
            var roleRepository = new RoleRepository(database);

            Settings = new AppSettingsService();
            AuthService = new AuthService(database, userRepository, passwordHasher);
            TicketService = new TicketService(ticketRepository);
            UserService = new UserService(userRepository);
            LookupService = new LookupService(priorityRepository, statusRepository, ticketTypeRepository, roleRepository);
        }
    }
