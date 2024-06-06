using System.Text.Json.Serialization;

namespace SocialApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Post Post { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
