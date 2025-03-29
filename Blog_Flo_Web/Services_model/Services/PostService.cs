using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.ViewModels.Tags;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.ViewModels.Posts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_Flo_Web.Services_model.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly ITagRepository _tagRepo;
        private readonly UserManager<User> _userManager;
        private readonly ICommentRepository _commentRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(
            IPostRepository repo,
            ITagRepository tagRepo,
            UserManager<User> userManager,
            ICommentRepository commentRepo,
            IMapper mapper,
            ILogger<PostService> logger)
        {
            _repo = repo;
            _tagRepo = tagRepo;
            _userManager = userManager;
            _commentRepo = commentRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public PostCreateViewModel CreatePost()
        {
            var post = new Post();
            var allTags = _tagRepo.GetAllTags().Select(t => new TagViewModel() { Id = t.Id, Name = t.Name }).ToList();
            var model = new PostCreateViewModel
            {
                Title = post.Title = string.Empty,
                Content = post.Content = string.Empty,
                Tags = allTags
            };
            return model;
        }

        public async Task<Guid> CreatePost(PostCreateViewModel model)
        {
            if (model.AuthorId == null)
            {
                _logger.LogWarning("AuthorId равен null");
                throw new ArgumentNullException(nameof(model.AuthorId), "AuthorId не может быть null");
            }

            var dbTags = new List<Tag>();
            if (model.Tags != null)
            {
                var postTags = model.Tags.Where(t => t.IsSelected == true).ToList();
                var tagsId = postTags.Select(t => t.Id).ToList();
                dbTags = _tagRepo.GetAllTags().Where(t => tagsId.Contains(t.Id)).ToList();
            }

            var post = new Post
            {
                Id = model.Id,
                Title = model.Title,
                Content = model.Content,
                Tags = dbTags,
                AuthorId = model.AuthorId
            };

            var user = await _userManager.FindByIdAsync(model.AuthorId);
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с id {model.AuthorId} не найден");
                throw new InvalidOperationException("Пользователь не найден");
            }

            user.Posts.Add(post);
            await _repo.AddPost(post);
            await _userManager.UpdateAsync(user);
            return post.Id;
        }

        public async Task<PostEditViewModel> EditPost(Guid id)
        {
            // Асинхронно получаем пост
            var post = await _repo.GetPostAsync(id); // Предполагается, что у вас есть асинхронный метод в репозитории
            if (post == null)
            {
                _logger.LogWarning($"Пост с id {id} не найден");
                throw new InvalidOperationException("Пост не найден");
            }

            // Асинхронно получаем теги
            var allTags = await _tagRepo.GetAllTagsAsync(); // Предполагается асинхронный метод
            var tags = allTags.Select(t => new TagViewModel()
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            var tagsToIterate = post.Tags ?? new List<Tag>();

            // Оптимизированное сравнение тегов
            var selectedTagIds = new HashSet<Guid>(tagsToIterate.Select(t => t.Id));
            foreach (var tag in tags)
            {
                tag.IsSelected = selectedTagIds.Contains(tag.Id);
            }

            return new PostEditViewModel()
            {
                Id = id,
                Title = post.Title,
                Content = post.Content,
                Tags = tags
            };
        }

        public async Task EditPost(PostEditViewModel model, Guid id)
        {
            if (model.Tags == null)
            {
                _logger.LogWarning("Список тегов равен null");
                throw new ArgumentNullException(nameof(model.Tags), "Список тегов не может быть null");
            }

            var post = _repo.GetPost(id);
            if (post == null)
            {
                _logger.LogWarning($"Пост с id {id} не найден");
                throw new InvalidOperationException("Пост не найден");
            }

            post.Title = model.Title;
            post.Content = model.Content;

            foreach (var tag in model.Tags)
            {
                var tagChanged = _tagRepo.GetTag(tag.Id);
                if (tag.IsSelected)
                {
                    if (tagChanged != null)
                    {
                        post.Tags.Add(tagChanged);
                    }
                }
                else
                {
                    if (tagChanged != null)
                    {
                        post.Tags.Remove(tagChanged);
                    }
                }
            }

            await _repo.UpdatePost(post);
        }

        public async Task RemovePost(Guid id)
        {
            await _repo.RemovePost(id);
            _logger.LogInformation($"Пост с id {id} удален");
        }

        public async Task<List<Post>> GetPosts()
        {
            // Используем асинхронную версию, если она есть в репозитории
            return await _repo.GetAllPostsAsync() ?? new List<Post>();
        }

        public async Task<Post> ShowPost(Guid id)
        {
            var post = _repo.GetPost(id);
            if (post == null)
            {
                _logger.LogWarning($"Пост с id {id} не найден");
                throw new InvalidOperationException("Пост не найден");
            }

            if (post.AuthorId == null)
            {
                _logger.LogWarning($"AuthorId не задан для поста с id {id}");
                throw new InvalidOperationException("AuthorId не задан");
            }

            var user = await _userManager.FindByIdAsync(post.AuthorId.ToString());
            if (user == null)
            {
                _logger.LogWarning($"Пользователь с id {post.AuthorId} не найден");
                throw new InvalidOperationException("Пользователь не найден");
            }

            var comments = _commentRepo.GetCommentsByPostId(post.Id) ?? new List<Comment>();
            post.Id = id;

            foreach (var comment in comments)
            {
                if (post.Comments.FirstOrDefault(c => c.Id == comment.Id) == null)
                {
                    post.Comments.Add(comment);
                }
            }

            if (!string.IsNullOrEmpty(user.UserName))
            {
                post.AuthorId = user.UserName;
            }
            else
            {
                post.AuthorId = "nonUsernamed";
            }

            return post;
        }

        public async Task<List<Post>> GetPostsByAuthor(string authorId)
        {
            return await _repo.GetPostsByAuthor(authorId);
        }
    }
}
