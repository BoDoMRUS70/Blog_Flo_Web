using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Business_model.Repositories.IRepositories
{
    public interface ITagRepository
    {
        HashSet<Tag> GetAllTags();
        Tag? GetTag(Guid id); // Добавлен nullable аннотация
        Task AddTag(Tag tag);
        Task UpdateTag(Tag tag);
        Task RemoveTag(Guid id);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Tag>> GetAllTagsAsync();
    }
}
