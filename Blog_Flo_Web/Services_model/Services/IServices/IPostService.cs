using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface IPostService
    {
        PostCreateViewModel CreatePost();
        Task<Guid> CreatePost(PostCreateViewModel model);
        Task<PostEditViewModel> EditPost(Guid id); // Асинхронный метод
        Task EditPost(PostEditViewModel model, Guid id);
        Task RemovePost(Guid id);
        Task<List<Post>> GetPosts();
        Task<Post> ShowPost(Guid id);
        Task<List<Post>> GetPostsByAuthor(string authorId);
    }
}
