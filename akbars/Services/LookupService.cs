using System.Collections.Generic;
using akbars.Models;
using akbars.Repositories;

namespace akbars.Services
{
    public class LookupService : ILookupService
    {
        private readonly IPriorityRepository _priorityRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IRoleRepository _roleRepository;

        public LookupService(
            IPriorityRepository priorityRepository,
            IStatusRepository statusRepository,
            ITicketTypeRepository ticketTypeRepository,
            IRoleRepository roleRepository)
        {
            _priorityRepository = priorityRepository;
            _statusRepository = statusRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _roleRepository = roleRepository;
        }

        public List<Priority> GetPriorities()
        {
            return _priorityRepository.GetPriorities();
        }

        public List<Status> GetStatuses()
        {
            return _statusRepository.GetStatuses();
        }

        public List<TicketType> GetTicketTypes()
        {
            return _ticketTypeRepository.GetTypes();
        }

        public List<Role> GetRoles()
        {
            return _roleRepository.GetRoles();
        }

        public void AddPriority(string name, int slaHours)
        {
            _priorityRepository.AddPriority(name, slaHours);
        }

        public void AddStatus(string name, string description)
        {
            _statusRepository.AddStatus(name, description);
        }

        public void AddTicketType(string name)
        {
            _ticketTypeRepository.AddType(name);
        }
    }
