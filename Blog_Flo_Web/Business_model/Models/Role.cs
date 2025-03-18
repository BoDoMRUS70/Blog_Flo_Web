using Microsoft.AspNetCore.Identity;

namespace Blog_Flo_Web.Business_model.Models
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }

        public List<User> Users { get; set; } = new();
    }
}
