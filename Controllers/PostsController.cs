using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        DatabaseContext _db;
        AuthService _auth;
        public PostsController(DatabaseContext context, AuthService auth)
        {
            _db = context;
            _auth = auth;
        }

        
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Post>> PostPost(TextData data)
        {
            var p = new Post { Author = _auth.User, Text = data.Text };
            _db.Posts.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPost), new { id = p.Id }, p);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _db.Posts.Include(p => p.Comments).ThenInclude(p => p.Author).ThenInclude(u => u.Avatar).Include(p => p.Author).ThenInclude(u => u.Avatar).FirstOrDefaultAsync(p=>p.Id==id);
            if (post == null) return NotFound();
            return post;
        }
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutPost(int id, TextData data)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();
            if (post.AuthorId != _auth.User.Id) return Forbid(); 

            try 
            {
                post.Text = data.Text;
                _db.Entry(post).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Posts.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return Ok();
        }
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null)  return NotFound();
            if (post.AuthorId != _auth.User.Id && !_auth.User.IsAdmin) return Forbid();

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
