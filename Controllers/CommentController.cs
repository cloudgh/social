using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;
using Microsoft.AspNetCore.Http;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        DatabaseContext _db;
        AuthService _auth;
        public CommentController(DatabaseContext context, AuthService auth)
        {
            _db = context;
            _auth = auth;
        }
        [Authorize]
        [HttpPost("{postId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> PostComment(int postId, [FromBody] TextData data)
        {
            var p = await _db.Posts.FindAsync(postId);
            if (p == null) return NotFound();

            var c = new Comment { Author = _auth.User, Text = data.Text, Post = p };
            _db.Comments.Add(c);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetComment), new { id = c.Id }, c);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var post = await _db.Comments.Include(c => c.Author).ThenInclude(u=>u.Avatar).Include(c=>c.Post).FirstOrDefaultAsync(c => c.Id == id);
            if (post == null) return NotFound();
            return post;
        }
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutComment(int id, TextData data)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null) return NotFound();
            if (comment.AuthorId != _auth.User.Id) return Forbid();

            try
            {
                comment.Text = data.Text;
                _db.Entry(comment).State = EntityState.Modified;
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
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _db.Comments.FindAsync(id);
            if (comment == null) return NotFound();
            if (comment.AuthorId != _auth.User.Id && !_auth.User.IsAdmin) return Forbid();

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
