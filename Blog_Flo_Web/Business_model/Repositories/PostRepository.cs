using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Flo_Web.Business_model.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<Post> GetAllPosts()
        {
            return GetBaseQuery().ToList();
        }

        public Post? GetPost(Guid id)
        {
            return GetBaseQuery().FirstOrDefault(p => p.Id == id);
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await GetBaseQuery().ToListAsync();
        }

        public async Task<Post?> GetPostAsync(Guid id)
        {
            return await GetBaseQuery().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddPost(Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            await _context.Posts.AddAsync(post);
            await SaveChangesAsync();
        }

        public async Task UpdatePost(Post post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            _context.Entry(post).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public async Task RemovePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post is not null)
            {
                _context.Posts.Remove(post);
                await SaveChangesAsync();
            }
        }

        public async Task<List<Post>> GetPostsByAuthor(string authorId)
        {
            if (string.IsNullOrWhiteSpace(authorId))
                throw new ArgumentException("Author ID cannot be empty", nameof(authorId));

            return await GetBaseQuery()
                .Where(p => p.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Failed to save changes to database", ex);
            }
        }

        public async Task<Post?> GetPostWithCommentsAsync(Guid id)
        {
            return await GetBaseQuery(withTags: false)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Post?> GetPostWithTagsAsync(Guid id)
        {
            return await GetBaseQuery(withComments: false)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        private IQueryable<Post> GetBaseQuery(bool withTags = true, bool withComments = true)
        {
            if (_context.Posts is null)
                throw new InvalidOperationException("Posts DbSet is not initialized");

            var query = _context.Posts.AsQueryable();

            if (withTags)
                query = query.Include(p => p.Tags ?? new List<Tag>());

            if (withComments)
                query = query.Include(p => p.Comments ?? new List<Comment>());

            return query.AsNoTracking();
        }
    }
}



