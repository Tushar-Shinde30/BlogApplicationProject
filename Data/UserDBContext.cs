using BMS.Models;
using Microsoft.EntityFrameworkCore;

namespace BMS.Data
{
    public class UserDBContext : DbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-0PNL03I;Initial Catalog=DatabaseBlog;Integrated Security=True;Trust Server Certificate=True");
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> BMSUsers { get; set; }

        public DbSet<Posts> BMSPosts { get; set; }
    }
}
