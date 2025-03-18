using Microsoft.Extensions.Hosting;

namespace Blog_Flo_Web.Business_model.Models
{
    public class Tag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;

        public List<Post> Posts { get; set; } = new();
    }
}
