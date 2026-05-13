using System.ComponentModel.DataAnnotations;
using akbars.Models;

namespace akbars.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Заполните краткое описание.")]
        [Display(Name = "Краткое описание")]
        public string ShortDescription { get; set; } = string.Empty;

        [Display(Name = "Подробное описание")]
        public string DetailedDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Выберите приоритет.")]
        [Display(Name = "Приоритет")]
        public int PriorityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Выберите тип заявки.")]
        [Display(Name = "Тип заявки")]
        public int TypeId { get; set; }

        public required List<Priority> Priorities { get; set; }

        public required List<TicketType> TicketTypes { get; set; }
    }
}
