using SocialApi.Models;

namespace SocialApi.ViewModels
{
    public class SearchResult
    {
        public List<User> Users { get; set; }
        public List<Post> Posts { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
