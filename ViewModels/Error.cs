using System.ComponentModel.DataAnnotations;

namespace SocialApi.ViewModels
{
    public class Error
    {
        [Required]
        public string Message { get; set; }
    }
}
