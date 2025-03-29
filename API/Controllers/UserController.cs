using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;
using Blog_Flo_Web.Business_model.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            UserManager<User> userManager,
            ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Авторизация аккаунта пользователя
        /// </summary>
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginViewModel model)
        {
            _logger.LogInformation("Запрос на авторизацию пользователя с email: {Email}", model?.Email);

            if (string.IsNullOrEmpty(model?.Email) || string.IsNullOrEmpty(model.Password))
            {
                _logger.LogWarning("Некорректный запрос: email или пароль не указаны.");
                throw new ArgumentNullException("Некорректный запрос");
            }

            try
            {
                var result = await _userService.Login(model);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Ошибка авторизации: неверный пароль или аккаунт не существует.");
                    throw new AuthenticationException("Введен неверный пароль или такого аккаунта не существует");
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                if (roles.Contains("Администратор"))
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "Администратор"));
                else
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, roles.First()));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Пользователь с email: {Email} успешно авторизован.", model.Email);
                return Ok("Авторизация прошла успешно.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при авторизации пользователя с email: {Email}", model.Email);
                return StatusCode(500, "Произошла ошибка при авторизации.");
            }
        }

        /// <summary>
        /// Добавление аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddAccount([FromBody] UserRegisterViewModel model)
        {
            _logger.LogInformation("Запрос на добавление нового пользователя.");

            if (model == null)
            {
                _logger.LogWarning("Модель пользователя не может быть null.");
                return BadRequest("Модель пользователя не может быть null.");
            }

            try
            {
                var result = await _userService.Register(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь успешно добавлен.");
                    return StatusCode(201, "Пользователь успешно добавлен.");
                }
                else
                {
                    _logger.LogWarning("Не удалось добавить пользователя.");
                    return StatusCode(400, "Не удалось добавить пользователя.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении пользователя.");
                return StatusCode(500, "Произошла ошибка при добавлении пользователя.");
            }
        }

        /// <summary>
        /// Редактирование аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPatch("EditUser")]
        public async Task<IActionResult> EditAccount([FromBody] UserEditViewModel model)
        {
            _logger.LogInformation("Запрос на редактирование пользователя с ID: {UserId}", model?.Id);

            if (model == null)
            {
                _logger.LogWarning("Модель пользователя не может быть null.");
                return BadRequest("Модель пользователя не может быть null.");
            }

            try
            {
                var result = await _userService.EditAccount(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь с ID: {UserId} успешно отредактирован.", model.Id);
                    return StatusCode(201, "Пользователь успешно отредактирован.");
                }
                else
                {
                    _logger.LogWarning("Не удалось отредактировать пользователя с ID: {UserId}.", model.Id);
                    return StatusCode(400, "Не удалось отредактировать пользователя.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании пользователя с ID: {UserId}.", model.Id);
                return StatusCode(500, "Произошла ошибка при редактировании пользователя.");
            }
        }

        /// <summary>
        /// Удаление аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpDelete("RemoveUser/{id}")]
        public async Task<IActionResult> RemoveAccount(Guid id)
        {
            _logger.LogInformation("Запрос на удаление пользователя с ID: {UserId}", id);

            try
            {
                await _userService.RemoveAccount(id);

                _logger.LogInformation("Пользователь с ID: {UserId} успешно удален.", id);
                return StatusCode(201, "Пользователь успешно удален.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя с ID: {UserId}.", id);
                return StatusCode(500, "Произошла ошибка при удалении пользователя.");
            }
        }

        /// <summary>
        /// Получение всех аккаунтов пользователей
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<List<User>>> GetAccounts()
        {
            _logger.LogInformation("Запрос на получение всех пользователей.");

            try
            {
                var users = await _userService.GetAccounts();

                _logger.LogInformation("Успешно получены все пользователи.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех пользователей.");
                return StatusCode(500, "Произошла ошибка при получении пользователей.");
            }
        }
    }
}
