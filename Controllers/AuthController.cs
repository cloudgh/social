using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialApi.Models;
using SocialApi.Services;
using SocialApi.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        DatabaseContext _db;
        AuthService _auth;
        public AuthController(DatabaseContext db, AuthService auth)
        {
            _db = db;
            _auth = auth;
        }

        /// <summary>
        /// Log in and get auth token
        /// </summary>
        /// <response code = "401" >If wrong login/password pair</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<Error>(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Token>> LogIn(LoginUser data)
        {
            var t= await _auth.GetToken(data.Username, data.Password);
            if (t is null) return Unauthorized(new Error{ Message="Wrong username/password pair"});
            return new Token { token = t };
        }
        /// <summary>
        /// Sign up user and get auth token
        /// </summary>
        /// <response code = "400" >If username already already taken</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType<Error>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Token>> SignUp(UserDTO data)
        {
            if (_db.Users.SingleOrDefault(u => u.Username == data.Username) is not null)
                return BadRequest(new Error{Message="User with this username already exists"});

            using SHA256 hash = SHA256.Create();
            var u = new User { FirstName = data.FirstName, LastName = data.LastName, Username = data.Username, Email = data.Email, Password = Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(data.Password))) };
            _db.Users.Add(u);
            await _db.SaveChangesAsync();
            return new Token { token = await _auth.GetToken(data.Username, data.Password) };
        }
    }
}
