using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;


namespace Blog_Flo_Web.Business_model.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Comment> GetAllComments()
        {
            return _context.Comments?.ToList() ?? new List<Comment>();
        }

        public Comment? GetComment(Guid id)
        {
            return _context.Comments?.FirstOrDefault(c => c.Id == id);
        }

        public List<Comment> GetCommentsByPostId(Guid id)
        {
            return _context.Comments?.Where(c => c.PostId == id).ToList() ?? new List<Comment>();
        }

        public async Task AddComment(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            _context.Comments?.Add(comment);
            await SaveChangesAsync();
        }

        public async Task UpdateComment(Comment comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            _context.Comments?.Update(comment);
            await SaveChangesAsync();
        }

        public async Task RemoveComment(Guid id)
        {
            var comment = GetComment(id);
            if (comment != null)
            {
                _context.Comments?.Remove(comment);
                await SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}