using Microsoft.AspNetCore.Identity;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Roles;
using Microsoft.EntityFrameworkCore;

namespace Blog_Flo_Web.Services_model.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Guid> CreateRole(RoleCreateViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var role = new Role()
            {
                Name = model.Name,
                Description = model.Description
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new Exception($"Role creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Guid.Parse(role.Id);
        }

        public async Task EditRole(RoleEditViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Если нечего обновлять - выходим
            if (string.IsNullOrEmpty(model.Name) && model.Description == null)
                return;

            var role = await _roleManager.FindByIdAsync(model.Id.ToString());

            if (role == null)
                throw new KeyNotFoundException($"Role with id {model.Id} not found");

            // Обновляем только измененные поля
            if (!string.IsNullOrEmpty(model.Name))
                role.Name = model.Name;

            if (model.Description != null)
                role.Description = model.Description;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new Exception($"Role update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task RemoveRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                throw new KeyNotFoundException($"Role with id {id} not found");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                throw new Exception($"Role deletion failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<Role?> GetRole(Guid id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }
    }
}
