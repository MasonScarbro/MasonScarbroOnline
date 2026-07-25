using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<MasonScarbroOnline.Models.Project> Projects { get; set; }
    }
}
