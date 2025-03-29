using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Получение всех ролей
        /// </summary>
        /// <remarks>
        /// Для получения всех ролей необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpGet("GetRoles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            _logger.LogInformation("Запрос на получение всех ролей.");

            try
            {
                var roles = await _roleService.GetRoles();

                _logger.LogInformation("Успешно получены все роли.");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех ролей.");
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }
        }

        /// <summary>
        /// Добавление роли
        /// </summary>
        /// <remarks>
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
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] RoleCreateViewModel model)
        {
            _logger.LogInformation("Запрос на добавление новой роли.");

            if (model == null)
            {
                _logger.LogWarning("Модель роли не может быть null.");
                return BadRequest("Модель роли не может быть null.");
            }

            try
            {
                await _roleService.CreateRole(model);

                _logger.LogInformation("Роль успешно добавлена.");
                return StatusCode(201, "Роль успешно добавлена.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении роли.");
                return StatusCode(500, "Произошла ошибка при добавлении роли.");
            }
        }

        /// <summary>
        /// Редактирование роли
        /// </summary>
        /// <remarks>
        /// Для редактирования роли необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPatch("EditRole")]
        public async Task<IActionResult> EditRole([FromBody] RoleEditViewModel model)
        {
            _logger.LogInformation("Запрос на редактирование роли с ID: {RoleId}", model?.Id);

            if (model == null)
            {
                _logger.LogWarning("Модель роли не может быть null.");
                return BadRequest("Модель роли не может быть null.");
            }

            try
            {
                await _roleService.EditRole(model);

                _logger.LogInformation("Роль с ID: {RoleId} успешно отредактирована.", model.Id);
                return StatusCode(201, "Роль успешно отредактирована.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании роли с ID: {RoleId}", model.Id);
                return StatusCode(500, "Произошла ошибка при редактировании роли.");
            }
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <remarks>
        /// Для удаления роли необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpDelete("RemoveRole/{id}")]
        public async Task<IActionResult> RemoveRole(Guid id)
        {
            _logger.LogInformation("Запрос на удаление роли с ID: {RoleId}", id);

            try
            {
                await _roleService.RemoveRole(id);

                _logger.LogInformation("Роль с ID: {RoleId} успешно удалена.", id);
                return StatusCode(201, "Роль успешно удалена.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении роли с ID: {RoleId}", id);
                return StatusCode(500, "Произошла ошибка при удалении роли.");
            }
        }
    }
}