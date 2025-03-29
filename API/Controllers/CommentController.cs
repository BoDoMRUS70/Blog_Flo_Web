using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentService commentService,
            UserManager<User> userManager,
            ILogger<CommentController> logger)
        {
            _commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Получение всех комментариев поста
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpGet("GetPostComment/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(Guid id)
        {
            _logger.LogInformation("Запрос на получение комментариев для поста с ID: {PostId}", id);

            try
            {
                var comments = await _commentService.GetComments();
                var postComments = comments.Where(c => c.PostId == id).ToList();

                _logger.LogInformation("Успешно получены комментарии для поста с ID: {PostId}", id);
                return Ok(postComments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении комментариев для поста с ID: {PostId}", id);
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }
        }

        /// <summary>
        /// Создание комментария к посту
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPost("CreateComment/{postId}")]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateViewModel model, Guid postId)
        {
            _logger.LogInformation("Запрос на создание комментария для поста с ID: {PostId}", postId);

            if (model == null)
            {
                _logger.LogWarning("Модель комментария не может быть null.");
                return BadRequest("Модель комментария не может быть null.");
            }

            model.PostId = postId;

            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);
                if (user == null)
                {
                    _logger.LogWarning("Пользователь не найден.");
                    return Unauthorized("Пользователь не найден.");
                }

                await _commentService.CreateComment(model, new Guid(user.Id));

                _logger.LogInformation("Комментарий успешно создан для поста с ID: {PostId}", postId);
                return StatusCode(201, "Комментарий успешно создан.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании комментария для поста с ID: {PostId}", postId);
                return StatusCode(500, "Произошла ошибка при создании комментария.");
            }
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPatch("EditComment")]
        public async Task<IActionResult> EditComment([FromBody] CommentEditViewModel model)
        {
            _logger.LogInformation("Запрос на редактирование комментария с ID: {CommentId}", model?.Id);

            if (model == null)
            {
                _logger.LogWarning("Модель комментария не может быть null.");
                return BadRequest("Модель комментария не может быть null.");
            }

            try
            {
                await _commentService.EditComment(model, model.Id);

                _logger.LogInformation("Комментарий с ID: {CommentId} успешно отредактирован.", model.Id);
                return StatusCode(201, "Комментарий успешно отредактирован.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании комментария с ID: {CommentId}", model.Id);
                return StatusCode(500, "Произошла ошибка при редактировании комментария.");
            }
        }

        /// <summary>
        /// Удаление комментария
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpDelete("RemoveComment/{id}")]
        public async Task<IActionResult> RemoveComment(Guid id)
        {
            _logger.LogInformation("Запрос на удаление комментария с ID: {CommentId}", id);

            try
            {
                await _commentService.RemoveComment(id);

                _logger.LogInformation("Комментарий с ID: {CommentId} успешно удален.", id);
                return StatusCode(201, "Комментарий успешно удален.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении комментария с ID: {CommentId}", id);
                return StatusCode(500, "Произошла ошибка при удалении комментария.");
            }
        }
    }
}
