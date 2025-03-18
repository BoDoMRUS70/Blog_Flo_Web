using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Tags
{
    public class TagViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        public bool IsSelected { get; set; }
    }
}
