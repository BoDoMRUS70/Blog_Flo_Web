using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blog_Flo_Web.Business_model.Models;

namespace Blog_Flo_Web.Business_model
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        /// Ссылка на таблицу Posts
        public DbSet<Post>? Posts { get; set; }
        /// Ссылка на таблицу Tags
        public DbSet<Tag>? Tags { get; set; }
        /// Ссылка на таблицу Comments
        public DbSet<Comment>? Comments { get; set; }
        /// Ссылка на таблицу Users
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Дополнительные настройки модели
        }
    }
}
