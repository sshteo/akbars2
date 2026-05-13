using System.ComponentModel.DataAnnotations;
using akbars.Models;

namespace akbars.ViewModels
{
    public class AssignTicketViewModel
    {
        public int TicketId { get; set; }

        public string TicketSummary { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите исполнителя.")]
        public int ExecutorId { get; set; }

        public required List<User> Executors { get; set; }
    }
}
