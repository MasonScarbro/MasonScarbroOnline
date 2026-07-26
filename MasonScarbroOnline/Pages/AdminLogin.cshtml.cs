using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace MasonScarbroOnline.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly PasswordHasher<object> _hasher = new();

        public AdminLoginModel(IConfiguration config)
        {
            _config = config;
        }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var storedPasswordHash = _config["Admin:PasswordHash"];
            if (string.IsNullOrEmpty(storedPasswordHash))
            {
                ErrorMessage = "Admin login isn't configured.";
                return Page();
            }
            var result = _hasher.VerifyHashedPassword(null!, storedPasswordHash, Password);
            if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                return RedirectToPage("/Admin/ProjectPages/Create");
            }
            else
            {
                ErrorMessage = "Invalid password.";
                return Page();
            }
        }
    }
}
