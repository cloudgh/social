using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        DatabaseContext _db;
        AuthService _auth;
        IWebHostEnvironment _env;
        public PhotosController(DatabaseContext context, AuthService auth, IWebHostEnvironment env)
        {
            _db = context;
            _auth = auth;
            _env = env;
        }


        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Photo>> PostPhoto(PhotoUpload data)
        {
            if (!Directory.Exists(_env.WebRootPath + "/images"))
                Directory.CreateDirectory(_env.WebRootPath + "/images");

            string path = $"/images/{_auth.User.Username}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}{Path.GetExtension(data.File.FileName)}";
            using (var fileStream = new FileStream(_env.WebRootPath + path, FileMode.Create))
                await data.File.CopyToAsync(fileStream);
            if(data.Description == null)
            {
                data.Description = "default";
            }
            var p = new Photo { Author = _auth.User, Url = path, Description = data.Description };
            _db.Photos.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPhoto), new { id = p.Id }, p);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Photo>> GetPhoto(int id)
        {
            var photo = await _db.Photos.Include(p => p.Author).ThenInclude(u => u.Avatar).FirstOrDefaultAsync(p => p.Id == id);
            if (photo == null) return NotFound();
            return photo;
        }


        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return NotFound();
            if (photo.AuthorId != _auth.User.Id && !_auth.User.IsAdmin) return Forbid();

            System.IO.File.Delete(_env.WebRootPath + photo.Url);
            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
