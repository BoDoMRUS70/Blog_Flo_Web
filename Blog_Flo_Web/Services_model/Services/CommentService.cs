using AutoMapper;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Comments;

namespace Blog_Flo_Web.Services_model.Services
{
    public class CommentService : ICommentService
    {
        public IMapper _mapper;
        private readonly ICommentRepository _commentRepo;//private readonly UserManager<User> _userManager;

        public CommentService(IMapper mapper, ICommentRepository commentRepo)
        {
            _mapper = mapper;
            _commentRepo = commentRepo;//_userManager = userManager;
        }

        public async Task<Guid> CreateComment(CommentCreateViewModel model, Guid userId)
        {
            var comment = new Comment
            {
                Content = model.Content,
                AuthorName = model.Author,
                PostId = model.PostId,//UserId = userId,//AuthorName = _userManager.FindByIdAsync(userId.ToString()).Result.UserName,
            };
            await _commentRepo.AddComment(comment);
            return comment.Id;
        }

        public async Task<CommentEditViewModel> EditComment(Guid id)
        {
            var comment = _commentRepo.GetComment(id);
            var result = new CommentEditViewModel
            {
                Content = comment.Content,
                Author = comment.AuthorName,
            };
            return result;
        }

        public async Task EditComment(CommentEditViewModel model, Guid id)
        {
            var comment = _commentRepo.GetComment(id);
            comment.Content = model.Content;
            comment.AuthorName = model.Author;
            await _commentRepo.UpdateComment(comment);
        }

        public async Task RemoveComment(Guid id)
        {
            await _commentRepo.RemoveComment(id);
        }

        public async Task<List<Comment>> GetComments()
        {
            return _commentRepo.GetAllComments().ToList();
        }
        public async Task<Comment> GetComment(Guid id)//
        {
            return _commentRepo.GetComment(id);
        }
    }
}
