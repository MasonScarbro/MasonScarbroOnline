using MasonScarbroOnline.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MasonScarbroOnline.Pages.Admin;
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    public DashboardModel(AppDbContext db) => _db = db;

    public List<DailyViews> ViewsLast30Days { get; set; } = new();
    public List<TopPath> TopPaths { get; set; } = new();
    public List<TopReferrer> TopReferrers { get; set; } = new();
    public int TotalViews { get; set; }

    public async Task OnGetAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);

        TotalViews = await _db.PageViews.CountAsync();

        ViewsLast30Days = await _db.PageViews
            .Where(v => v.ViewedAt >= cutoff)
            .GroupBy(v => v.ViewedAt.Date)
            .Select(g => new DailyViews { Date = g.Key, Count = g.Count() })
            .OrderBy(d => d.Date)
            .ToListAsync();

        TopPaths = await _db.PageViews
            .GroupBy(v => v.Path)
            .Select(g => new TopPath { Path = g.Key, Count = g.Count() })
            .OrderByDescending(p => p.Count)
            .Take(10)
            .ToListAsync();

        TopReferrers = await _db.PageViews
            .Where(v => v.Referrer != null)
            .GroupBy(v => v.Referrer)
            .Select(g => new TopReferrer { Referrer = g.Key!, Count = g.Count() })
            .OrderByDescending(r => r.Count)
            .Take(10)
            .ToListAsync();
    }

    public record DailyViews { public DateTime Date; public int Count; }
    public record TopPath { public string Path = ""; public int Count; }
    public record TopReferrer { public string Referrer = ""; public int Count; }
}