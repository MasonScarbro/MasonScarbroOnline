using MasonScarbroOnline.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;

namespace MasonScarbroOnline.Pages
{
    [EnableRateLimiting("ask-policy")]
    public class AskModel : PageModel
    {
        private readonly IExperienceQAService _qa;

        public AskModel(IExperienceQAService qa)
        {
            _qa = qa;
        }

        [BindProperty]
        public string Question { get; set; } = string.Empty;

        public string? Answer { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Question))
                return;

            Answer = await _qa.AskAsync(Question);
        }
    }
}