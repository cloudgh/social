using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;

namespace SocialApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class MainController : ControllerBase
    {
        DatabaseContext _db;
        AuthService _auth;
        public MainController(DatabaseContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }
        /// <summary>
        /// Get posts feed
        /// </summary>
        [HttpGet("feed")]
        public async Task<IEnumerable<Post>> Feed()
        {
            return await _db.Posts
                .Include(p=>p.Author).ThenInclude(u => u.Avatar)
                .Include(p=>p.Comments).ThenInclude(c=>c.Author).ThenInclude(u => u.Avatar)
                //.Where(p=>p.Author!=_auth.User)
                .OrderByDescending(p => p.Created)
                .ToListAsync();
        }
        /// <summary>
        /// Universal search for users, posts and photos
        /// </summary>
        [HttpGet("search")]
        public async Task<SearchResult> Feed(string query)
        {
            query = query.ToLower();
            return new SearchResult
            {
                Posts = await _db.Posts.Include(o=>o.Author).ThenInclude(u => u.Avatar).Where(p=> p.Text.ToLower().Contains(query)).ToListAsync(),
                Users = await _db.Users.Include(u => u.Avatar).Where(u=> (u.FirstName+u.LastName+u.Username).ToLower().Contains(query)).ToListAsync(),
                Photos = await _db.Photos.Include(o => o.Author).ThenInclude(u => u.Avatar).Where(p => p.Description.ToLower().Contains(query)).ToListAsync(),
            };
        }
    }
}
