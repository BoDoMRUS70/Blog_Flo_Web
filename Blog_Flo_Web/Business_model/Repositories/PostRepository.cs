using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Blog_Flo_Web.Business_model.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.Include(p => p.Tags).ToList();
        }

        public Post GetPost(Guid id)
        {
            return _context.Posts.Include(p => p.Tags).FirstOrDefault(p => p.Id == id);
        }

        public async Task AddPost(Post post)
        {
            _context.Posts.Add(post);
            await SaveChangesAsync();
        }

        public async Task UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            await SaveChangesAsync();
        }

        public async Task RemovePost(Guid id)
        {
            var post = GetPost(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await SaveChangesAsync();
            }
        }
        public async Task<List<Post>> GetPostsByAuthor(string authorId)//
        {
            return await _context.Posts
                .Include(p => p.Tags)
                .Where(p => p.AuthorId == authorId)
                .ToListAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
