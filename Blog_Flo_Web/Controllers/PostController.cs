using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, UserManager<User> userManager, ILogger<PostController> logger)
        {
            _postService = postService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// [Get] Метод, показа поста
        /// </summary>
        [Route("Post/Show")]
        [HttpGet]
        public async Task<IActionResult> ShowPost(Guid id)
        {
            var post = await _postService.ShowPost(id);

            return View(post);
        }

        /// <summary>
        /// [Get] Метод, создания поста
        /// </summary>
        [Route("Post/Create")]
        [HttpGet]
        [Authorize]
        public IActionResult CreatePost()
        {
            var model = _postService.CreatePost();
            return View(model);
        }

        /// <summary>
        /// [Post] Метод, создания поста
        /// </summary>
        [Route("Post/Create")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostCreateViewModel model)
        {
            // Проверка имени пользователя
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogWarning("Имя пользователя не найдено");
                return RedirectToAction("Login", "Account"); // Перенаправление на страницу входа
            }

            // Поиск пользователя
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с именем {userName} не найден");
                return NotFound("Пользователь не найден");
            }

            // Установка автора поста
            model.AuthorId = user.Id;

            // Проверка заполнения обязательных полей
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("", "Не все поля заполнены");
                _logger.LogError("Пост не создан, ошибка при создании - Не все поля заполнены");
                return View(model);
            }

            // Создание поста
            await _postService.CreatePost(model);
            _logger.LogInformation($"Создан пост - {model.Title}");

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Get] Метод, редактирования поста
        /// </summary>
        [Route("Post/Edit")]
        [HttpGet]
        public async Task<IActionResult> EditPost(Guid id)
        {
            var model = await _postService.EditPost(id);

            return View(model);
        }

        /// <summary>
        /// [Post] Метод, редактирования поста
        /// </summary>
        [Authorize]
        [Route("Post/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditPost(PostEditViewModel model, Guid Id)
        {
            if (string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content))
            {
                ModelState.AddModelError("", "Не все поля заполнены");
                _logger.LogError("Пост не отредактирован, ошибка при редактировании - Не все поля заполнены");

                return View(model);
            }
            await _postService.EditPost(model, Id);
            _logger.LogInformation($"Пост {model.Title} отредактирован");

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Get] Метод, удаления поста
        /// </summary>
        [HttpGet]
        [Route("Post/Remove")]
        public async Task<IActionResult> RemovePost(Guid id, bool confirm = true)
        {
            if (confirm)
                await RemovePost(id);

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Post] Метод, удаления поста
        /// </summary>
        [HttpPost]
        [Route("Post/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        public async Task<IActionResult> RemovePost(Guid id)
        {
            await _postService.RemovePost(id);
            _logger.LogInformation($"Пост с id {id} удален");

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Get] Метод, получения всех постов
        /// </summary>
        [HttpGet]
        [Route("Post/Get")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postService.GetPosts();

            return View(posts);
        }

        [HttpGet]
        [Route("Post/GetByAuthor/{authorId}")]
        public async Task<IActionResult> GetPostsByAuthor(string authorId)
        {
            var posts = await _postService.GetPostsByAuthor(authorId);
            return View(posts);
        }
    }
}
