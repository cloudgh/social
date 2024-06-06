using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        DatabaseContext _db;
        AuthService _auth;
        IWebHostEnvironment _env;
        public UsersController(DatabaseContext context, AuthService auth, IWebHostEnvironment env)
        {
            _db = context;
            _auth = auth;
            _env = env;
        }

        /// <summary>
        /// Get my profile
        /// </summary>
        [Authorize()]
        [HttpGet("me")]
        public async Task<User> GetMeAsync()
        {
            
            return await _db.Users
                .Include(u => u.Posts).ThenInclude(p => p.Comments).ThenInclude(c=>c.Author).ThenInclude(u=>u.Avatar)
                .Include(u => u.Photos).FirstAsync(u=>u.Id==_auth.User.Id);
        }
        /// <summary>
        /// Edit my profile
        /// </summary>
        /// <response code = "200" >Succesfully updated</response>
        [Authorize()]
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMe(UserDTO data)
        {
            if (_db.Users.SingleOrDefault(u => u.Username == data.Username && u.Id!=_auth.User.Id) is not null)
                return BadRequest(new Error { Message = "User with this username already exists" });
            var ph = data.AvatarId!=null ? _db.Photos.Find(data.AvatarId): null;
            if (data.AvatarId is not null && ph is null) return BadRequest(new Error { Message = "Avatar with such id not found" });

            _auth.User.FirstName = data.FirstName;
            _auth.User.LastName = data.LastName;
            _auth.User.Username = data.Username;
            _auth.User.Email = data.Email;
            _auth.User.Avatar = ph;
            using SHA256 hash = SHA256.Create();
            _auth.User.Password = Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(data.Password)));

            await _db.SaveChangesAsync();
            return Ok();
        }


        /// <summary>
        /// Get user profile by id
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _db.Users
                .Include(u=>u.Posts).ThenInclude(p=>p.Comments).ThenInclude(c => c.Author).ThenInclude(u => u.Avatar)
                .Include(u=>u.Photos)
                .FirstOrDefaultAsync(u=>u.Id==id);
            if (user == null) return NotFound();
            return user;
        }
        /// <summary>
        /// Get user profile by username
        /// </summary>
        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            var user = await _db.Users
                .Include(u => u.Posts).ThenInclude(p => p.Comments).ThenInclude(c => c.Author).ThenInclude(u => u.Avatar)
                .Include(u => u.Photos).FirstOrDefaultAsync(u=>u.Username==username);
            if (user == null) return NotFound();
            return user;
        }
    }
}
