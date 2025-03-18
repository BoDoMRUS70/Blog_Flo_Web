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
    [Route("controller")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        /// <summary>
        /// Авторизация аккаунта пользователя
        /// </summary>
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate(UserLoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                throw new ArgumentNullException("Некорректный запрос");

            var result = await _userService.Login(model);
            if (!result.Succeeded)
                throw new AuthenticationException("Введен неверный пароль или такого аккаунта не существует");

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

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            return StatusCode(200);
        }

        /// <summary>
        /// Добавление аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddAccount(UserRegisterViewModel model)
        {
            var result = await _userService.Register(model);

            return StatusCode(result.Succeeded ? 201 : 204);
        }

        /// <summary>
        /// Редактирование аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPatch]
        [Route("EditUser")]
        public async Task<IActionResult> EditAccount(UserEditViewModel model)
        {
            var result = await _userService.EditAccount(model);

            return StatusCode(result.Succeeded ? 201 : 204);
        }

        /// <summary>
        /// Удаление аккаунта пользователя
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpDelete]
        [Route("RemoveUser")]
        public async Task<IActionResult> RemoveAccount(Guid id)
        {
            await _userService.RemoveAccount(id);

            return StatusCode(201);
        }

        /// <summary>
        /// Получение всех аккаунтов пользователей
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [Route("GetUsers")]
        public Task<List<User>> GetAccounts()
        {
            var users = _userService.GetAccounts();

            return Task.FromResult(users.Result);
        }
    }
}
