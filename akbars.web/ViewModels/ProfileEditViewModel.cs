using System.ComponentModel.DataAnnotations;

namespace akbars.ViewModels
{
    public class ProfileEditViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите фамилию.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите имя.")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Телефон")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Отдел")]
        public string Department { get; set; } = string.Empty;
    }
}
