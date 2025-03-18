using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Users
{
    public class UserLoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Введите email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string? Password { get; set; }
    }
}
