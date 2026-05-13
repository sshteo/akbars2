using System.ComponentModel.DataAnnotations;

namespace akbars.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин.")]
        [Display(Name = "Логин")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        public string SystemStatus { get; set; } = string.Empty;

        public bool SystemAvailable { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
