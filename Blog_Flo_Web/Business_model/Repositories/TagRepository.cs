using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Flo_Web.Business_model.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public HashSet<Tag> GetAllTags()
        {
            return _context.Tags?.ToHashSet() ?? new HashSet<Tag>();
        }

        // Исправленная асинхронная версия метода
        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            if (_context.Tags == null)
                return Enumerable.Empty<Tag>();

            return await _context.Tags.ToListAsync();
        }

        public Tag? GetTag(Guid id)
        {
            return _context.Tags?.FirstOrDefault(t => t.Id == id);
        }

        public async Task AddTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            _context.Tags?.Add(tag);
            await SaveChangesAsync();
        }

        public async Task UpdateTag(Tag tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            _context.Tags?.Update(tag);
            await SaveChangesAsync();
        }

        public async Task RemoveTag(Guid id)
        {
            var tag = GetTag(id);
            if (tag != null)
            {
                _context.Tags?.Remove(tag);
                await SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
