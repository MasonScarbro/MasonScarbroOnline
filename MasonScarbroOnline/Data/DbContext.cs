using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<MasonScarbroOnline.Models.Project> Projects { get; set; }
        public DbSet<MasonScarbroOnline.Models.Experience> Experiences { get; set; }
        public DbSet<MasonScarbroOnline.Models.Thought> Thoughts { get; set; }
        public DbSet<MasonScarbroOnline.Models.TidBit> TidBits { get; set; }
        public DbSet<MasonScarbroOnline.Models.PageView> PageViews { get; set; }
    }
}
