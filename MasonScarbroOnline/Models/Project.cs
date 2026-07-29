using MasonScarbroOnline.Helpers;
using System.ComponentModel.DataAnnotations;
namespace MasonScarbroOnline.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Display(Prompt = "C#, ASP.NET Core, EF Core...")]
        public string TechStack { get; set; } = string.Empty;

        [DataType(DataType.Url)] public string? ImageUrl { get; set; }
        [DataType(DataType.Url)] public string? RepoUrl { get; set; }
        [DataType(DataType.Url)] public string? DemoUrl { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
