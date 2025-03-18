using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Business_model.Repositories.IRepositories
{
    public interface ITagRepository
    {
        HashSet<Tag> GetAllTags();

        Tag GetTag(Guid id);

        Task AddTag(Tag tag);

        Task UpdateTag(Tag tag);

        Task RemoveTag(Guid id);

        Task<bool> SaveChangesAsync();
    }
}
