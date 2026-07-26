using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages.Admin.ExperiencePages
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            _context.Attach(Experience).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Experiences.Any(e => e.Id == Experience.Id))
                    return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}