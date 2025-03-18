using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface IPostService
    {
        Task<PostCreateViewModel> CreatePost();

        Task<Guid> CreatePost(PostCreateViewModel model);

        Task<PostEditViewModel> EditPost(Guid Id);

        Task EditPost(PostEditViewModel model, Guid Id);

        Task RemovePost(Guid id);

        Task<List<Post>> GetPosts();

        Task<Post> ShowPost(Guid id);

        Task<List<Post>> GetPostsByAuthor(string authorId);//
    }
}
