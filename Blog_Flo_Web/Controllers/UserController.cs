using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Users;

namespace Blog_Flo_Web.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class UserController : Controller
    {
        private readonly IUserService _userService;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// [Get] Метод, login
        /// </summary>
        [Route("Account/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// [Post] Метод, login
        /// </summary>
        [Route("Account/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Login(model);

                if (result.Succeeded)
                {
					Logger.Info($"Осуществлен вход пользователя с адресом - {model.Email}");
					return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        /// <summary>
        /// [Get] Метод, создания пользователя
        /// </summary>
        [Route("User/Create")]
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        /// <summary>
        /// [Post] Метод, создания пользователя
        /// </summary>
        [Route("User/Create")]
        [HttpPost]
        public async Task<IActionResult> AddUser(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUser(model);

                if (result.Succeeded)
                {
					Logger.Info($"Создан аккаунт, пользователем с правами администратора, с использованием адреса - {model.Email}");
					return RedirectToAction("GetAccounts", "User");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// [Get] Метод, регистрации
        /// </summary>
        [Route("User/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// [Post] Метод, регистрации
        /// </summary>
        [Route("User/Register")]
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.Register(model);

                if (result.Succeeded)
                {
					Logger.Info($"Создан аккаунт с использованием адреса - {model.Email}");
					return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// [Get] Метод, редактирования аккаунта
        /// </summary>
        [Route("User/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> EditAccount(Guid id)
        {
            var model = await _userService.EditAccount(id);

            return View(model);
        }

        /// <summary>
        /// [Post] Метод, редактирования аккаунта
        /// </summary>
        [Route("User/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> EditAccount(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.EditAccount(model);
				Logger.Info($"Аккаунт {model.UserName} был изменен");

				return RedirectToAction("GetAccounts", "User");
            }

            else
            {
                return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, удаление аккаунта
        /// </summary>
        [Route("User/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> RemoveAccount(Guid id, bool confirm = true)
        {
            if (confirm)

                await RemoveAccount(id);

            return RedirectToAction("GetAccounts", "User");
        }

        /// <summary>
        /// [Post] Метод, удаление аккаунта
        /// </summary>
        [Route("User/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> RemoveAccount(Guid id)
        {
            var account = await _userService.GetAccount(id);
            await _userService.RemoveAccount(id);
			Logger.Info($"Аккаунт с id - {id} удален");

			return RedirectToAction("GetAccounts", "User");
        }

        /// <summary>
        /// [Post] Метод, выхода из аккаунта
        /// </summary>
        [Route("User/Logout")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogoutAccount()
        {
            await _userService.LogoutAccount();
			Logger.Info($"Осуществлен выход из аккаунта");

			return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// [Get] Метод, получения всех пользователей
        /// </summary>
        [Route("User/Get")]
        [Authorize(Roles = "Администратор, Модератор")]
        public async Task<IActionResult> GetAccounts()
        {
            var users = await _userService.GetAccounts();

            return View(users);
        }

        /// <summary>
        /// [Get] Метод, получения одного пользователя по Id
        /// </summary>
        [Route("User/Details")]
        [Authorize(Roles = "Администратор, Модератор")]
        public async Task<IActionResult> DetailsAccount(Guid id)
        {
            var model = await _userService.GetAccount(id);

            return View(model);
        }
    }
}
