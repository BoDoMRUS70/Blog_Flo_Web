using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Comments;
using Blog_Flo_Web.Business_model.Models;


namespace Blog_Flo_Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, UserManager<User> userManager, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _userManager = userManager;
            _logger = logger;
        }

        // <summary>
        /// [Get] Метод, добавление комментария
        /// </summary>
        [HttpGet]
        [Route("Comment/CreateComment")]
        public IActionResult CreateComment(Guid postId)
        {
            var model = new CommentCreateViewModel() { PostId = postId };

            return View(model);
        }

        // <summary>
        /// [Post] Метод, добавление комментария
        /// </summary>
        [HttpPost]
        [Route("Comment/CreateComment")]
        public async Task<IActionResult> CreateComment(CommentCreateViewModel model, Guid postId)
        {
            model.PostId = postId;

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

            // Создание комментария
            var post = _commentService.CreateComment(model, new Guid(user.Id));
            _logger.LogInformation($"Пользователь {model.Author} добавил комментарий к статье {postId}");

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Get] Метод, редактирования коментария
        /// </summary>
        [Route("Comment/Edit")]
        [HttpGet]
        public async Task<IActionResult> EditComment(Guid id)
        {
            var view = await _commentService.EditComment(id);

            return View(view);
        }

        /// <summary>
        /// [Post] Метод, редактирования коментария
        /// </summary>
        [Authorize]
        [Route("Comment/Edit")]
        [HttpPost]
        public async Task<IActionResult> EditComment(CommentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _commentService.EditComment(model, model.Id);
                _logger.LogInformation($"Пользователь {model.Author} изменил комментарий {model.Id}");

                return RedirectToAction("GetPosts", "Post");
            }
            else
            {
                ModelState.AddModelError("", "Некорректные данные");

                return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, удаления коментария
        /// </summary>
        [HttpGet]
        [Route("Comment/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        public async Task<IActionResult> RemoveComment(Guid id, bool confirm = true)
        {
            if (confirm)
                await RemoveComment(id);

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Delete] Метод, удаления коментария
        /// </summary>
        [HttpDelete]
        [Route("Comment/Remove")]
        public async Task<IActionResult> RemoveComment(Guid id)
        {
            await _commentService.RemoveComment(id);
            _logger.LogInformation($"Комментарий с id {id} удален");

            return RedirectToAction("GetPosts", "Post");
        }

        /// <summary>
        /// [Get] Метод, получения всех тегов
        /// </summary>
        [Route("Comment/Get")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            var comments = await _commentService.GetComments();

            return View(comments);
        }

        public async Task<IActionResult> DetailsComment(Guid id)
        {
            var comments = await _commentService.GetComment(id);

            return View(comments);
        }
    }
}
