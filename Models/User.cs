using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; } = false;
        public Photo? Avatar { get; set; }

        [JsonIgnore]
        public string Password { get; set; } = null!;

        public List<Post> Posts { get; set; } = new();
        [JsonIgnore]
        public List<Comment> Comments { get; set; } = new();
        public List<Photo> Photos { get; set; } = new();
    }
    public class UserDTO
    {
        [Required(ErrorMessage = "Enter your first name")]
        public required string FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username lenght must be from 3 to 20")]
        public required string Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password lenght must be from 8 to 20")]
        public required string Password { get; set; } = null!;

        [DefaultValue(null)]
        public int? AvatarId { get; set; } = null;
    }
    public class LoginUser
    {
        [Required()]
        //[StringLength(20, MinimumLength = 3, ErrorMessage = "Username lenght must be from 3 to 20")]
        public string Username { get; set; }

        [Required()]
        //[StringLength(20, MinimumLength = 8, ErrorMessage = "Password lenght must be from 8 to 20")]
        public string Password { get; set; }
    }
}
