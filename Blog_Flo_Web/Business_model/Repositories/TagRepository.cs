using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;


namespace Blog_Flo_Web.Business_model.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public HashSet<Tag> GetAllTags()
        {
            return _context.Tags.ToHashSet();
        }

        public Tag GetTag(Guid id)
        {
            return _context.Tags.FirstOrDefault(t => t.Id == id);
        }

        public async Task AddTag(Tag tag)
        {
            _context.Tags.Add(tag);
            await SaveChangesAsync();
        }

        public async Task UpdateTag(Tag tag)
        {
            _context.Tags.Update(tag);
            await SaveChangesAsync();
        }

        public async Task RemoveTag(Guid id)
        {
            _context.Tags.Remove(GetTag(id));
            await SaveChangesAsync();
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
