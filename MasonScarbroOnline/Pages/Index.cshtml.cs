using MasonScarbroOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Data.AppDbContext _context;

        public IndexModel(Data.AppDbContext context)
        {
            _context = context;
        }

        public List<Project> Projects { get; set; } = new();
        public List <Experience> Experiences { get; set; } = new();
        public async Task OnGetAsync()
        {
            Projects = await _context.Projects
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            Experiences = await _context.Experiences
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}
