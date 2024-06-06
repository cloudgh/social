using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocialApi.Services
{
    public class AuthService
    {
        public User? User { set; get; } = null;
        DatabaseContext _db;
        IConfiguration _cfg;
        public AuthService(IHttpContextAccessor ctx, DatabaseContext db, IConfiguration configuration)
        {
            _db = db;
            _cfg = configuration;
            if (ctx.HttpContext?.User.Identity?.Name != null)
                User = _db.Users.Find(int.Parse(ctx.HttpContext.User.Identity.Name));
        }
        public async Task<string?> GetToken(string username, string password)
        {
            using SHA256 hash = SHA256.Create();
            var user = await _db.Users.SingleOrDefaultAsync(x =>
                x.Username == username &&
                x.Password == Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(password)))
            );
            if (user == null) return null;

            var jwt = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: new List<Claim> { new Claim(ClaimTypes.Name, user.Id.ToString()) },
                expires: DateTime.UtcNow.AddDays(14),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"])), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
