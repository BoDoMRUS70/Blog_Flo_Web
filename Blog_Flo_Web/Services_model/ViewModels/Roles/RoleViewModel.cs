using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Roles
{
    public class RoleViewModel
    {
        public string? Id { get; set; } // Идентификатор роли (допускает null)
        public string? Name { get; set; } // Название роли (допускает null)
        public bool IsSelected { get; set; } // Флаг, указывающий, выбрана ли роль
    }
}