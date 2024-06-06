using Microsoft.EntityFrameworkCore;
using SocialApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SocialApi.Services
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            if(Database.EnsureCreated())
            {
                Users.Add(new User { FirstName="Admin", Username="admin", IsAdmin=true, Password=Hash("admin") });
                SaveChanges();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasOne(u => u.Avatar).WithMany(p => p.UsersWithAvatar);
            modelBuilder.Entity<User>().HasMany(u => u.Photos).WithOne(p => p.Author);
        }
        public string Hash(string input)
        {
            using SHA256 hash = SHA256.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(input)));
        } 
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
