using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SocialApi.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        public int AuthorId { get; set; }

        public User Author { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }
}
