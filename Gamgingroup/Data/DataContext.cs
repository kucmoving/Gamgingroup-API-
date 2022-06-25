using Gamgingroup.Models;
using Microsoft.EntityFrameworkCore;

namespace Gamgingroup.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Photo> Photos { get; set; }
    }
}
