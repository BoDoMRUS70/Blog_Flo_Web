using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.Services;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Blog_Flo_Web.Business_model.Repositories;
using Blog_Flo_Web.Business_model;

namespace Blog_Flo_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("Blog_Flo_Web.DAL")))
                .AddIdentity<User, Role>(opts =>
                {
                    opts.Password.RequiredLength = 5;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                    opts.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            var mapperConfig = new MapperConfiguration((v) =>
            {
                v.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            // регистрация сервисов репозитория для взаимодействия с базой данных
            builder.Services.AddSingleton(mapper)
                .AddTransient<ICommentRepository, CommentRepository>()
                .AddTransient<IPostRepository, PostRepository>()
                .AddTransient<ITagRepository, TagRepository>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<ICommentService, CommentService>()
                .AddTransient<IHomeService, HomeService>()
                .AddTransient<IPostService, PostService>()
                .AddTransient<IRoleService, RoleService>()
                .AddTransient<ITagService, TagService>()
                .AddControllersWithViews();

            // подключение logger
            builder.Logging.ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error"); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}