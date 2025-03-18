using AutoMapper;
using Blog_Flo_Web.Business_model.Models;
using Blog_Flo_Web.Business_model.Repositories.IRepositories;
using Blog_Flo_Web.Business_model.Repositories;
using Blog_Flo_Web.Business_model;
using Blog_Flo_Web.Services_model.Services.IServices;
using Blog_Flo_Web.Services_model.Services;
using Blog_Flo_Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Final task blog API",
                    Description = "Final task blog",
                });
                var basePath = AppContext.BaseDirectory;

                var xmlPath = Path.Combine(basePath, "API.xml");
                options.IncludeXmlComments(xmlPath);
            });

            var mapperConfig = new MapperConfiguration((v) =>
            {
                v.AddProfile(new MappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            var assembly = Assembly.GetAssembly(typeof(MappingProfile));

            builder.Services.AddAutoMapper(assembly);

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection))
                .AddIdentity<User, Role>(opts =>
                {
                    opts.Password.RequiredLength = 5;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            // Services AddSingletons,Transient
            builder.Services
                .AddSingleton(mapper)
                .AddTransient<IUserService, UserService>()
                .AddTransient<ICommentService, CommentService>()
                .AddTransient<ICommentRepository, CommentRepository>()
                .AddTransient<IPostService, PostService>()
                .AddTransient<IPostRepository, PostRepository>()
                .AddTransient<IRoleService, RoleService>()
                .AddTransient<ITagService, TagService>()
                .AddTransient<ITagRepository, TagRepository>();


            builder.Services.AddAuthentication(optionts => optionts.DefaultScheme = "Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}