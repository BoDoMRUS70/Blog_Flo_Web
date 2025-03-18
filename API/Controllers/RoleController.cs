using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Получение всех ролей
        /// </summary>
        /// <remarks>
        /// Для получения всех ролей необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [Route("GetRoles")]
        public async Task<IEnumerable<Role>> GetRoles()
        {
            var roles = await _roleService.GetRoles();

            return roles;
        }

        /// <summary>
        /// Добавление роли
        /// </summary>
        /// <remarks>
        /// 
        /// Для добавления роли необходимы права администратора
        /// 
        /// Пример запроса:
        ///
        ///     POST /Todo
        ///     {
        ///        "name": "SuperUser",
        ///        "description": "VIP User with extended access rights"
        ///     }
        ///
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        [Route("AddRole")]
        public async Task<IActionResult> AddRole(RoleCreateViewModel model)
        {
            await _roleService.CreateRole(model);

            return StatusCode(201);
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        /// <remarks>
        /// Для редактирования роли необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPatch]
        [Route("EditRole")]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            await _roleService.EditRole(model);

            return StatusCode(201);
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <remarks>
        /// Для удаления роли необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpDelete]
        [Route("RemoveRole")]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            await _roleService.RemoveRole(id);

            return StatusCode(201);
        }
    }
}
