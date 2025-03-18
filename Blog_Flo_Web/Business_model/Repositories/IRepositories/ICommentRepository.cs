using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Business_model.Repositories.IRepositories
{
    public interface ICommentRepository
    {
        List<Comment> GetAllComments();

        Comment GetComment(Guid id);

        List<Comment> GetCommentsByPostId(Guid id);

        Task AddComment(Comment item);

        Task UpdateComment(Comment item);

        Task RemoveComment(Guid id);

        Task<bool> SaveChangesAsync();
    }
}
