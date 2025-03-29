using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Получение всех тегов
        /// </summary>
        /// <remarks>
        /// Для получения всех тегов необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpGet("GetTags")]
        public async Task<ActionResult<List<Tag>>> GetTags()
        {
            _logger.LogInformation("Запрос на получение всех тегов.");

            try
            {
                var tags = await _tagService.GetTags();

                _logger.LogInformation("Успешно получены все теги.");
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех тегов.");
                return StatusCode(500, "Произошла ошибка при обработке запроса.");
            }
        }

        /// <summary>
        /// Добавление тега
        /// </summary>
        /// <remarks>
        /// Для добавления тега необходимы права администратора
        /// 
        /// Пример запроса:
        ///
        ///     POST /Todo
        ///     {
        ///        "name": "#.Net",
        ///     }
        ///
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPost("AddTag")]
        public async Task<IActionResult> AddTag([FromBody] TagCreateViewModel model)
        {
            _logger.LogInformation("Запрос на добавление нового тега.");

            if (model == null)
            {
                _logger.LogWarning("Модель тега не может быть null.");
                return BadRequest("Модель тега не может быть null.");
            }

            try
            {
                var tagId = await _tagService.CreateTag(model);

                if (tagId != Guid.Empty)
                {
                    _logger.LogInformation("Тег успешно добавлен с ID: {TagId}", tagId);
                    return StatusCode(201, new { TagId = tagId, Message = "Тег успешно добавлен." });
                }
                else
                {
                    _logger.LogWarning("Не удалось добавить тег.");
                    return StatusCode(400, "Не удалось добавить тег.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении тега.");
                return StatusCode(500, "Произошла ошибка при добавлении тега.");
            }
        }

        /// <summary>
        /// Редактирование тега
        /// </summary>
        /// <remarks>
        /// Для редактирования тега необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpPatch("EditTag")]
        public async Task<IActionResult> EditTag([FromBody] TagEditViewModel model)
        {
            _logger.LogInformation("Запрос на редактирование тега с ID: {TagId}", model?.Id);

            if (model == null)
            {
                _logger.LogWarning("Модель тега не может быть null.");
                return BadRequest("Модель тега не может быть null.");
            }

            try
            {
                await _tagService.EditTag(model, model.Id);

                _logger.LogInformation("Тег с ID: {TagId} успешно отредактирован.", model.Id);
                return StatusCode(201, "Тег успешно отредактирован.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при редактировании тега с ID: {TagId}", model.Id);
                return StatusCode(500, "Произошла ошибка при редактировании тега.");
            }
        }

        /// <summary>
        /// Удаление тега
        /// </summary>
        /// <remarks>
        /// Для удаления тега необходимы права администратора
        /// </remarks>
        [Authorize(Roles = "Администратор")]
        [HttpDelete("RemoveTag/{id}")]
        public async Task<IActionResult> RemoveTag(Guid id)
        {
            _logger.LogInformation("Запрос на удаление тега с ID: {TagId}", id);

            try
            {
                await _tagService.RemoveTag(id);

                _logger.LogInformation("Тег с ID: {TagId} успешно удален.", id);
                return StatusCode(201, "Тег успешно удален.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении тега с ID: {TagId}", id);
                return StatusCode(500, "Произошла ошибка при удалении тега.");
            }
        }
    }
}

