using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialApi.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; } = "default";
        public DateTime Created { get; set; } = DateTime.Now;
        public int AuthorId { get; set; }
        public User Author { get; set; }

        [JsonIgnore]
        public List<User> UsersWithAvatar { get; set; }
    }
    public class PhotoUpload
    {
        public string? Description { get; set; } = "default";

        [Required]
        public IFormFile File { get; set; }
    }
}
