using Blog_Flo_Web.Services_model.ViewModels.Roles;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Services_model.Services.IServices
{
    public interface IRoleService
    {
        Task<Guid> CreateRole(RoleCreateViewModel model);

        Task EditRole(RoleEditViewModel model);

        Task RemoveRole(Guid id);

        Task<List<Role>> GetRoles();

        Task<Role?> GetRole(Guid id);
    }
}
