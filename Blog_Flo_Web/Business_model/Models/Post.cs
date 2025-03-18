using System.Xml.Linq;

namespace Blog_Flo_Web.Business_model.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

        public DateTime CreatedData { get; set; } = DateTime.UtcNow;
        public string? AuthorId { get; set; }
        public User? User { get; set; }
        public List<Tag> Tags { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }
}
