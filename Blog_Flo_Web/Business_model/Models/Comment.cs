namespace Blog_Flo_Web.Business_model.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public DateTime DateAdd { get; set; }
        public string? AuthorName { get; set; }
        public User? User { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
    }
}
