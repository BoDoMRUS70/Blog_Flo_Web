using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Roles
{
    public class RoleEditViewModel
    {
        public Guid Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Название", Prompt = "Название")]
        public string? Name { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Описание роли", Prompt = "описание")]
        public string? Description { get; set; } = null;
    }
}
