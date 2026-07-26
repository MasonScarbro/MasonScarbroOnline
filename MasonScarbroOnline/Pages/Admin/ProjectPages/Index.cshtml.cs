using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MasonScarbroOnline.Models;
using MasonScarbroOnline.Data;

namespace MasonScarbroOnline.Pages.ProjectPages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Project> Project { get; set; } = default!;

    public async Task OnGetAsync()
    {
        Project = await _context.Projects.ToListAsync();
    }
}
