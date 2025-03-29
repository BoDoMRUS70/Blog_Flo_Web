using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.ViewModels.Comments;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface ICommentService
    {
        Task<Guid> CreateComment(CommentCreateViewModel model, Guid userId);
        Task EditComment(CommentEditViewModel model, Guid id);
        Task RemoveComment(Guid id);
        Task<List<Comment>> GetComments(); // Возвращаемый тип Task<List<Comment>>
        Task<Comment?> GetComment(Guid id); // Возвращаемый тип Task<Comment>
        Task<CommentEditViewModel?> EditComment(Guid id);
    }
}
