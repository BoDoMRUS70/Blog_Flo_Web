using AutoMapper;
using Blog_Flo_Web.Middleware;
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

            // ��������� ����������� � ���� ������
            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connection, b => b.MigrationsAssembly("Blog_Flo_Web.DAL")))
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

            // ��������� AutoMapper
            var mapperConfig = new MapperConfiguration((v) =>
            {
                v.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            // ����������� �������� �����������
            builder.Services
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

            // ��������� �����������
            builder.Logging.ClearProviders()
                .SetMinimumLevel(LogLevel.Trace)
                .AddConsole();

            var app = builder.Build();

            // ��������� ���������� ���������� ����������
            app.UseMiddleware<ExceptionMiddleware>();

            // ������������ HTTP-���������
            if (!app.Environment.IsDevelopment())
            {
                // � ������ ���������� �������������� �� �������� ������
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // �������� HSTS (HTTP Strict Transport Security)
            }
            else
            {
                // � ������ ���������� ���������� ���������������� ������
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); // ��������������� HTTP �� HTTPS
            app.UseStaticFiles(); // ��������� ����������� ������
            app.UseRouting(); // �������������
            app.UseAuthentication(); // ��������������
            app.UseAuthorization(); // �����������

            // ��������� ������ HTTP (404, 500 � �.�.)
            app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

            // ��������� ��������� �� ���������
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}