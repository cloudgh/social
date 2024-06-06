using System.ComponentModel.DataAnnotations;

namespace SocialApi.ViewModels
{
    public class TextData
    {
        [Required]
        [StringLength(512, MinimumLength = 3)]
        public required string Text { get; set; }
    }
}
