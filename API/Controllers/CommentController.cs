using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;

        public CommentController(ICommentService commentService, UserManager<User> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Получение всех комментариев поста
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpGet]
        [Route("GetPostComment")]
        public async Task<IEnumerable<Comment>> GetComments(Guid id)
        {
            var comment = await _commentService.GetComments();

            return comment.Where(c => c.PostId == id);
        }

        /// <summary>
        /// Создание комментария к посту
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPost]
        [Route("CreateComment")]
        public async Task<IActionResult> CreateComment(CommentCreateViewModel model, Guid postId)
        {
            model.PostId = postId;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            await _commentService.CreateComment(model, new Guid(user.Id));

            return StatusCode(201);
        }

        /// <summary>
        /// Редактирование комментария
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpPatch]
        [Route("EditComment")]
        public async Task<IActionResult> EditComment(CommentEditViewModel model)
        {
            await _commentService.EditComment(model, model.Id);

            return StatusCode(201);
        }

        /// <summary>
        /// Удаление комментария
        /// </summary>
        [Authorize(Roles = "Администратор")]
        [HttpDelete]
        [Route("RemoveComment")]
        public async Task<IActionResult> RemoveComment(Guid id)
        {
            await _commentService.RemoveComment(id);

            return StatusCode(201);
        }
    }
}
