using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;

namespace MasonScarbroOnline.Middleware
{
    public class PageViewTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public PageViewTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // AppDbContext is resolved per-request here, not in the constructor —
        // this is what lets a singleton-lifetime middleware safely use a scoped service.
        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            string path = context.Request.Path.Value ?? "/";

            bool isTracked = !path.StartsWith("/admin", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("/css") && !path.StartsWith("/js")
                && !path.StartsWith("/images") && !path.StartsWith("/lib")
                && Path.GetExtension(path) == string.Empty;

            if (isTracked && context.Request.Method == "GET")
            {
                db.PageViews.Add(new PageView
                {
                    Path = path,
                    Referrer = context.Request.Headers.Referer.ToString() is { Length: > 0 } r ? r : null
                });
                await db.SaveChangesAsync();
            }

            await _next(context);
        }
    }
}