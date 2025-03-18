using Blog_Flo_Web.Services_model.ViewModels.Comments;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface ICommentService
    {
        Task<Guid> CreateComment(CommentCreateViewModel model, Guid userId);

        Task RemoveComment(Guid id);

        Task<List<Comment>> GetComments();
        Task<Comment> GetComment(Guid id);//

        Task<CommentEditViewModel> EditComment(Guid id);

        Task EditComment(CommentEditViewModel model, Guid id);
    }
}
