using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json.Serialization;

namespace MasonScarbroOnline.Services.Github
{
    public class GitHubStatsService
    {
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly string _username;

        public GitHubStatsService(HttpClient httpClient, IMemoryCache cache, IConfiguration config)
        {
            _http = httpClient;
            _cache = cache;
            _username = "MasonScarbro";

            _http.BaseAddress = new Uri("https://api.github.com/");
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("MasonScarbroOnline");
            var token = config["GitHub:Token"];
            if (!string.IsNullOrEmpty(token))
                _http.DefaultRequestHeaders.Authorization = new("Bearer", token);
        }

        public async Task<GitHubStatsSnapshot> GetSnapshotAsync()
        {
            return await _cache.GetOrCreateAsync("github-stats", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6);

                var repos = await _http.GetFromJsonAsync<List<GhRepo>>($"users/{_username}/repos?per_page=100") ?? new();
                var langTotals = new Dictionary<string, long>();
                int totalCommits = 0;
                int linesContributed = 0;

                foreach (var repo in repos.Where(r => !r.Fork))
                {
                    var langs = await _http.GetFromJsonAsync<Dictionary<string, long>>($"repos/{_username}/{repo.Name}/languages") ?? new();
                    foreach (var (lang, bytes) in langs)
                        langTotals[lang] = langTotals.GetValueOrDefault(lang) + bytes;

                    var contributors = await GetContributorStatsSafeAsync(repo.Name);
                    var mine = contributors?.FirstOrDefault(c => c.Author?.Login == _username);
                    if (mine != null)
                    {
                        totalCommits += mine.Total;
                        linesContributed += mine.Weeks.Sum(w => w.Additions);
                    }
                }

                var totalBytes = langTotals.Values.Sum();
                var topLanguages = langTotals
                    .OrderByDescending(l => l.Value)
                    .Take(5)
                    .Select(l => (l.Key, totalBytes == 0 ? 0 : Math.Round(l.Value * 100.0 / totalBytes, 1)))
                    .ToList();

                return new GitHubStatsSnapshot
                {
                    TotalRepos = repos.Count,
                    TotalCommits = totalCommits,
                    LinesContributed = linesContributed,
                    TopLanguages = topLanguages,
                    GeneratedAt = DateTime.UtcNow
                };

            }) ?? new GitHubStatsSnapshot();
        }
        private async Task<List<GhContributorStats>?> GetContributorStatsSafeAsync(string repoName)
        {
            for (int attempt = 0; attempt < 6; attempt++)
            {
                var response = await _http.GetAsync($"repos/{_username}/{repoName}/stats/contributors");

                if (response.StatusCode == HttpStatusCode.Accepted)
                {
                    await Task.Delay(4000);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                    return null;

                try
                {
                    return await response.Content.ReadFromJsonAsync<List<GhContributorStats>>();
                }
                catch (System.Text.Json.JsonException)
                {
                    return null;
                }
            }

            return null;
        }
    }

    public class GhRepo { public string Name { get; set; } = ""; public bool Fork { get; set; } }
    public class GhContributorStats { public GhAuthor? Author { get; set; } public int Total { get; set; } public List<GhWeek> Weeks { get; set; } = new(); }
    public class GhAuthor { public string? Login { get; set; } }
    public class GhWeek
    {
        [JsonPropertyName("w")] public long Week { get; set; }
        [JsonPropertyName("a")] public int Additions { get; set; }
        [JsonPropertyName("d")] public int Deletions { get; set; }
        [JsonPropertyName("c")] public int Commits { get; set; }
    }
}