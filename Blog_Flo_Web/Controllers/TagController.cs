using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Tags;

namespace Blog_Flo_Web.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class TagController : Controller
    {
        private readonly ITagService _tagService;
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
		
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// [Get] Метод, создания тега
        /// </summary>
        [Route("Tag/Create")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public IActionResult CreateTag()
        {
            return View();
        }

        /// <summary>
        /// [Post] Метод, создания тега
        /// </summary>
        [Route("Tag/Create")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> CreateTag(TagCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tagId = _tagService.CreateTag(model);
				Logger.Info($"Создан тег - {model.Name}");

				return RedirectToAction("GetTags", "Tag");
            }
            else
            {
				Logger.Error($"Ошибка при создании тега - {model.Name}");

				return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, редактирования тега
        /// </summary>
        [Route("Tag/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> EditTag(Guid id)
        {
            var view = await _tagService.EditTag(id);

            return View(view);
        }

        /// <summary>
        /// [Post] Метод, редактирования тега
        /// </summary>
        [Route("Tag/Edit")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> EditTag(TagEditViewModel model, Guid id)
        {
            if (ModelState.IsValid)
            {
                await _tagService.EditTag(model, id);
				Logger.Info($"Изменен тег - {model.Name}");

				return RedirectToAction("GetTags", "Tag");
            }
            else
            {
				Logger.Error($"Ошибка при изменении тега - {model.Name}");

				return View(model);
            }
        }

        /// <summary>
        /// [Get] Метод, удаления тега
        /// </summary>
        [Route("Tag/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> RemoveTag(Guid id, bool isConfirm = true)
        {
            if (isConfirm)
                await RemoveTag(id);

            return RedirectToAction("GetTags", "Tag");
        }

        /// <summary>
        /// [Post] Метод, удаления тега
        /// </summary>
        [Route("Tag/Remove")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpPost]
        public async Task<IActionResult> RemoveTag(Guid id)
        {
            var tag = await _tagService.GetTag(id);
            await _tagService.RemoveTag(id);
			Logger.Info($"Удаленн тег - {id}");

			return RedirectToAction("GetTags", "Tag");
        }

        /// <summary>
        /// [Get] Метод, получения всех тегов
        /// </summary>
        [Route("Tag/Get")]
        [Authorize(Roles = "Администратор, Модератор")]
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _tagService.GetTags();

            return View(tags);
        }

		/// <summary>
		/// [Get] Метод, просмотра данных о теге
		/// </summary>
		[Route("Tag/Details")]
		[Authorize(Roles = "Администратор, Модератор")]
		[HttpGet]
		public async Task<IActionResult> DetailsTag(Guid id)
        {
            var tags = await _tagService.GetTag(id);

            return View(tags);
        }
    }
}
