using MasonScarbroOnline.Data;
using MasonScarbroOnline.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

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