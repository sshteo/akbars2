using System;
using System.Collections.Generic;
using akbars.Models;
using akbars.Repositories;

namespace akbars.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public TicketStatistics GetStatistics(int? authorId, int? assigneeId)
        {
            return _ticketRepository.GetStatistics(authorId, assigneeId);
        }

        public List<TicketListItem> GetTickets(TicketQuery query)
        {
            return _ticketRepository.GetTickets(query);
        }

        public TicketDetails GetTicketDetails(int ticketId)
        {
            return _ticketRepository.GetTicketDetails(ticketId);
        }

        public int CreateTicket(int authorId, string shortDescription, string detailedDescription, int priorityId, int typeId)
        {
            if (string.IsNullOrWhiteSpace(shortDescription))
            {
                throw new InvalidOperationException("Заполните краткое описание заявки.");
            }

            if (priorityId <= 0 || typeId <= 0)
            {
                throw new InvalidOperationException("Выберите тип и приоритет заявки.");
            }

            return _ticketRepository.CreateTicket(new Ticket
            {
                AuthorId = authorId,
                ShortDescription = shortDescription.Trim(),
                DetailedDescription = string.IsNullOrWhiteSpace(detailedDescription) ? string.Empty : detailedDescription.Trim(),
                PriorityId = priorityId,
                TypeId = typeId
            });
        }

        public void AssignTicket(int ticketId, int executorId, int changedByUserId)
        {
            _ticketRepository.AssignTicket(ticketId, executorId, changedByUserId);
        }

        public void StartTicket(int ticketId, int changedByUserId, string note)
        {
            _ticketRepository.UpdateTicketStatus(ticketId, 2, changedByUserId, note);
        }

        public void CompleteTicket(int ticketId, int changedByUserId, string note)
        {
            _ticketRepository.UpdateTicketStatus(ticketId, 3, changedByUserId, note);
        }

        public void CancelTicket(int ticketId, int changedByUserId, string note)
        {
            _ticketRepository.UpdateTicketStatus(ticketId, 4, changedByUserId, note);
        }

        public bool DeleteTicket(int ticketId, int authorId)
        {
            return _ticketRepository.DeleteTicket(ticketId, authorId);
        }
    }
}
