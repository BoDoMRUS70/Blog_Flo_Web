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
        private readonly ICommentRepository _commentRepo;

        public CommentService(IMapper mapper, ICommentRepository commentRepo)
        {
            _mapper = mapper;
            _commentRepo = commentRepo;
        }

        public async Task<Guid> CreateComment(CommentCreateViewModel model, Guid userId)
        {
            var comment = new Comment
            {
                Content = model.Content,
                AuthorName = model.Author,
                PostId = model.PostId,
            };
            await _commentRepo.AddComment(comment);
            return comment.Id;
        }

        public Task<CommentEditViewModel?> EditComment(Guid id)
        {
            var comment = _commentRepo.GetComment(id);

            if (comment == null)
            {
                return Task.FromResult<CommentEditViewModel?>(null);
            }

            var result = new CommentEditViewModel
            {
                Content = comment.Content,
                Author = comment.AuthorName,
            };

            return Task.FromResult<CommentEditViewModel?>(result);
        }

        public async Task EditComment(CommentEditViewModel model, Guid id)
        {
            var comment = _commentRepo.GetComment(id);

            if (comment == null)
            {
                throw new ArgumentException("Комментарий не найден", nameof(id));
            }

            comment.Content = model.Content;
            comment.AuthorName = model.Author;

            await _commentRepo.UpdateComment(comment);
        }

        public async Task RemoveComment(Guid id)
        {
            await _commentRepo.RemoveComment(id);
        }

        public Task<List<Comment>> GetComments()
        {
            var comments = _commentRepo.GetAllComments().ToList();
            return Task.FromResult(comments);
        }

        public Task<Comment?> GetComment(Guid id)
        {
            var comment = _commentRepo.GetComment(id);
            return Task.FromResult(comment);
        }
    }
}
