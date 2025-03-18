using System.ComponentModel.DataAnnotations;

namespace Blog_Flo_Web.Services_model.ViewModels.Comments
{
    public class CommentEditViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Содержание", Prompt = "Содержание")]
        public string? Content { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Автор", Prompt = "Автор")]
        public string? Author { get; set; }

        public Guid Id { get; set; }
    }
}
