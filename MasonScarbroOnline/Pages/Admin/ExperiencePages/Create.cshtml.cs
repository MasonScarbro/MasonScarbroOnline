using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MasonScarbroOnline.Pages.Admin.ExperiencePages
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Experience Experience { get; set; } = default!;

        public IActionResult OnGet() => Page();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Experiences.Add(Experience);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}