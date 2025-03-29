using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Services_model.Services.IServices;

namespace Blog_Flo_Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly ILogger<HomeController> _logger;
        private readonly IHostEnvironment _env;

        public HomeController(
            IHomeService homeService,
            ILogger<HomeController> logger,
            IHostEnvironment env)
        {
            _homeService = homeService;
            _logger = logger;
            _env = env; // Добавляем IHostEnvironment для проверки режима разработки
        }

        public async Task<IActionResult> Index()
        {
            await _homeService.GenerateData();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Home/Error")]
        public IActionResult Error(int? statusCode = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                IsDevelopment = _env.IsDevelopment(), // Проверяем, находится ли приложение в режиме разработки
                StatusCode = statusCode // Добавляем код статуса в модель
            };

            if (statusCode.HasValue)
            {
                if (statusCode == 400 || statusCode == 403 || statusCode == 404)
                {
                    var viewName = statusCode.ToString();
                    _logger.LogError($"Произошла ошибка - {statusCode}\n{viewName}");
                    return View(viewName, errorViewModel); // Передаем модель в представление
                }
                return View("400", errorViewModel); // Передаем модель в представление
            }

            return View("400", errorViewModel); // Передаем модель в представление
        }

        //generate error 400
        [Route("GetException400")]
        [HttpGet]
        public IActionResult GetException400()
        {
            try
            {
                throw new HttpRequestException("400");
            }
            catch
            {
                return View("400", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    IsDevelopment = _env.IsDevelopment(),
                    StatusCode = 400 // Указываем код статуса
                });
            }
        }

        //generate error 403
        [Route("GetException403")]
        [HttpGet]
        public IActionResult GetException403()
        {
            try
            {
                throw new HttpRequestException("403");
            }
            catch
            {
                return View("403", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    IsDevelopment = _env.IsDevelopment(),
                    StatusCode = 403 // Указываем код статуса
                });
            }
        }

        //generate error 404
        [Route("GetException404")]
        [HttpGet]
        public IActionResult GetException404()
        {
            try
            {
                throw new HttpRequestException("404");
            }
            catch
            {
                return View("404", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    IsDevelopment = _env.IsDevelopment(),
                    StatusCode = 404 // Указываем код статуса
                });
            }
        }
    }
}