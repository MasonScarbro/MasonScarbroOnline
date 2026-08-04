using MasonScarbroOnline.Data;
using MasonScarbroOnline.Middleware;
using MasonScarbroOnline.Models;
using MasonScarbroOnline.Services;
using MasonScarbroOnline.Services.Github;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null)));
builder.Services.AddHttpClient<IExperienceQAService, ExperienceQAService>();
builder.Services.AddHttpClient<GitHubStatsService>(); 
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/AdminLogin";
        options.AccessDeniedPath = "/AdminLogin";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddRazorPages(options =>
{
   
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/AdminLogin");
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.UseMiddleware<PageViewTrackingMiddleware>();
app.MapRazorPages()
   .WithStaticAssets();

app.MapPost("/api/ask", async (AskRequest req, IExperienceQAService qa) =>
{
    if (string.IsNullOrWhiteSpace(req.Question))
        return Results.BadRequest(new { error = "Question is required." });

    var answer = await qa.AskAsync(req.Question);
    return Results.Ok(new { answer });
})
.RequireRateLimiting("ask-policy");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Projects.Any())
    {
        db.Projects.AddRange(
            new Project
            {
                Title = "Portfolio Website",
                Description = "This very site — ASP.NET Core, EF Core, Docker.",
                TechStack = "C#, ASP.NET Core, EF Core, SQL Server, Tailwind",
                RepoUrl = "https://github.com/yourusername/MasonScarbroOnline",
                CreatedAt = DateTime.UtcNow
            },
            new Project
            {
                Title = "Test Project",
                Description = "A second row just to confirm the list renders properly.",
                TechStack = "C#, EF Core",
                CreatedAt = DateTime.UtcNow
            }
        );
        
        db.SaveChanges();
    }
    if (!db.Experiences.Any())
    {
        db.Experiences.AddRange(
            new Experience
            {
                Title = "Software Engineer",
                Description = "Developed web applications using ASP.NET Core and EF Core.",
                Location = "Remote",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2023, 1, 1)
            }
        );
        db.SaveChanges();
    }
}

app.Run();

record AskRequest(string Question);