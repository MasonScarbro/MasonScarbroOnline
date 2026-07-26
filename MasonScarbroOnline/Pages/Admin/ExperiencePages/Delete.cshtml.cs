using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages.Admin.ExperiencePages
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var experience = await _context.Experiences.FirstOrDefaultAsync(e => e.Id == id);
            if (experience == null) return NotFound();

            Experience = experience;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var experience = await _context.Experiences.FindAsync(id);
            if (experience != null)
            {
                _context.Experiences.Remove(experience);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}