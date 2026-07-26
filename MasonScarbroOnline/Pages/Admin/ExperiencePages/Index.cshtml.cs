using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages.Admin.ExperiencePages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Experience> Experience { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Experience = await _context.Experiences
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}