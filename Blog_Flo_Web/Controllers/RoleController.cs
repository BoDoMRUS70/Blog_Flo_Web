using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Roles;

namespace Blog_Flo_Web.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// [Get] Метод, создания тега
        /// </summary>
        [Route("Role/Create")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        /// <summary>
        /// [Post] Метод, создания тега
        /// </summary>
        [Route("Role/Create")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleId = await _roleService.CreateRole(model);
				Logger.Info($"Созданна роль - {model.Name}");

				return RedirectToAction("GetRoles", "Role");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
				Logger.Error($"Роль {model.Name} не создана, ошибка при создании - Некорректные данные");

				return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, редактирования тега
        /// </summary>
        [Route("Role/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> EditRole(Guid id)
        {
            var role = _roleService.GetRole(id);
            var view = new RoleEditViewModel { Id = id, Description = role.Result?.Description, Name = role.Result?.Name };

            return View(view);
        }

        /// <summary>
        /// [Post] Метод, редактирования тега
        /// </summary>
        [Route("Role/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _roleService.EditRole(model);
				Logger.Info($"Измененна роль - {model.Name}");

				return RedirectToAction("GetRoles", "Role");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");
				Logger.Error($"Роль {model.Name} не изменена, ошибка при изменении - Некорректные данные");

				return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, удаления тега
        /// </summary>
        [Route("Role/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> RemoveRole(Guid id, bool isConfirm = true)
        {
            if (isConfirm)
                await RemoveRole(id);

            return RedirectToAction("GetRoles", "Role");
        }

        /// <summary>
        /// [Post] Метод, удаления тега
        /// </summary>
        [Route("Role/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            await _roleService.RemoveRole(id);
			Logger.Info($"Удаленна роль - {id}");

			return RedirectToAction("GetRoles", "Role");
        }

        /// <summary>
        /// [Get] Метод, получения всех тегов
        /// </summary>
        [Route("Role/GetRoles")]
        [HttpGet]
        [Authorize(Roles = "Администратор, Модератор")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetRoles();

            return View(roles);
        }
    }
}
