using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MasonScarbroOnline.Models;
using MasonScarbroOnline.Data;

namespace MasonScarbroOnline.Pages.ProjectPages;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Project Project { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == id);
        if (project is null)
        {
            return NotFound();
        }
        else
        {
            Project = project;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            Project = project;
            _context.Projects.Remove(Project);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
