using Blog_Flo_Web.Services_model.ViewModels.Tags;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface ITagService
    {
        Task<Guid> CreateTag(TagCreateViewModel model);

        Task<TagEditViewModel> EditTag(Guid id);

        Task EditTag(TagEditViewModel model, Guid id);

        Task RemoveTag(Guid id);

        Task<List<Tag>> GetTags();

        Task<Tag> GetTag(Guid id);
    }
}
