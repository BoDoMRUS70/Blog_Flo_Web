using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Roles
{
    public class CommentViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
