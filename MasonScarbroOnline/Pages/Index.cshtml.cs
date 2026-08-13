using MasonScarbroOnline.Models;
using MasonScarbroOnline.Services.Github;
using MasonScarbroOnline.Services.SynthLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Data.AppDbContext _context;
        private readonly GitHubStatsService _gitHubStats;

        private readonly ChordStreamingService _chordStreamingService;
        public IndexModel(Data.AppDbContext context, GitHubStatsService gitHubStats, ChordStreamingService chordStreamingService)
        {
            _context = context;
            _gitHubStats = gitHubStats;
            _chordStreamingService = chordStreamingService;
        }

        public List<Project> Projects { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();
        public GitHubStatsSnapshot GitHubStats { get; set; } = new();
        public async Task OnGetAsync()
        {
            Projects = await _context.Projects
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            Experiences = await _context.Experiences
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
            GitHubStats = await _gitHubStats.GetSnapshotAsync();

            
        }
    }
}
