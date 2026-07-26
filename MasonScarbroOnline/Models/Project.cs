using MasonScarbroOnline.Helpers;
namespace MasonScarbroOnline.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TechStack { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? RepoUrl { get; set; }
        public string? DemoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
    }
}
