using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MasonScarbroOnline.Models;
using MasonScarbroOnline.Data;

namespace MasonScarbroOnline.Pages.ProjectPages;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;
    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

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
}
