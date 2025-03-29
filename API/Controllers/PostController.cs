using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Получение всех постов
        /// </summary>
        [HttpGet("GetPosts")]
        public async Task<ActionResult<IEnumerable<ShowPostViewModel>>> GetPosts()
        {
            _logger.LogInformation("Запрос на получение всех постов.");

            try
            {
                var posts = await _postService.GetPosts();
                var postsResponse = posts.Select(p => new ShowPostViewModel
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    Title = p.Title,
                    Content = p.Content,
                    Tags = p.Tags.Select(_ => _.Name)
                }).ToList();

                _logger.LogInformation("Успешно получены все посты.");
                return Ok(postsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех постов.");
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }
        }

        /// <summary>
        /// Добавление поста
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///        "authorId": "Tester",
        ///        "tags": [
        ///          {
        ///            "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///            "name": "#dotnet",
        ///            "isSelected": true
        ///          }
        ///         ],
        ///        "title": ".Net developer",
        ///        "content": "Something about the best job..."
        ///     }
        ///
        /// </remarks>
        [HttpPost("AddPost")]
        public async Task<IActionResult> AddPost([FromBody] PostCreateViewModel model)
        {
            _logger.LogInformation("Запрос на добавление нового поста.");

            if (model == null)
            {
                _logger.LogWarning("Модель поста не может быть null.");
                return BadRequest("Модель поста не может быть null.");
            }

            try
            {
                await _postService.CreatePost(model);

                _logger.LogInformation("Пост успешно добавлен.");
                return StatusCode(201, "Пост успешно добавлен.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении поста.");
                return StatusCode(500, "Произошла ошибка при добавлении поста.");
            }
        }

        /// <summary>
        /// Редактирование поста
        /// </summary>
        /// <remarks>
        /// Для редактирования поста необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPatch("EditPost")]
        public async Task<IActionResult> EditPost([FromBody] PostEditViewModel model)
        {
            _logger.LogInformation("Запрос на редактирование поста с ID: {PostId}", model?.Id);

            if (model == null)
            {
                _logger.LogWarning("Модель поста не может быть null.");
                return BadRequest("Модель поста не может быть null.");
            }

            try
            {
                await _postService.EditPost(model, model.Id);

                _logger.LogInformation("Пост с ID: {PostId} успешно отредактирован.", model.Id);
                return StatusCode(201, "Пост успешно отредактирован.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании поста с ID: {PostId}", model.Id);
                return StatusCode(500, "Произошла ошибка при редактировании поста.");
            }
        }

        /// <summary>
        /// Удаление поста
        /// </summary>
        /// <remarks>
        /// Для удаления поста необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpDelete("RemovePost/{id}")]
        public async Task<IActionResult> RemovePost(Guid id)
        {
            _logger.LogInformation("Запрос на удаление поста с ID: {PostId}", id);

            try
            {
                await _postService.RemovePost(id);

                _logger.LogInformation("Пост с ID: {PostId} успешно удален.", id);
                return StatusCode(201, "Пост успешно удален.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении поста с ID: {PostId}", id);
                return StatusCode(500, "Произошла ошибка при удалении поста.");
            }
        }
    }
}

