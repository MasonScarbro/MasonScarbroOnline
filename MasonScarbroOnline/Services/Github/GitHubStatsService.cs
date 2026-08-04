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
                var nonForkRepos = repos.Where(r => !r.Fork).ToList();

                using var throttle = new SemaphoreSlim(5);
                var perRepoResults = await Task.WhenAll(nonForkRepos.Select(async repo =>
                {
                    await throttle.WaitAsync();
                    try { return await GetRepoStatsAsync(repo.Name); }
                    finally { throttle.Release(); }
                }));

                var langTotals = new Dictionary<string, long>();
                int totalCommits = 0;
                int linesContributed = 0;

                foreach (var result in perRepoResults)
                {
                    foreach (var (lang, bytes) in result.Languages)
                        langTotals[lang] = langTotals.GetValueOrDefault(lang) + bytes;
                    totalCommits += result.Commits;
                    linesContributed += result.Lines;
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

        private record RepoStatsResult(Dictionary<string, long> Languages, int Commits, int Lines);

        private async Task<RepoStatsResult> GetRepoStatsAsync(string repoName)
        {
            var langsTask = _http.GetFromJsonAsync<Dictionary<string, long>>($"repos/{_username}/{repoName}/languages");
            var contributorsTask = GetContributorStatsSafeAsync(repoName);
            await Task.WhenAll(langsTask, contributorsTask);

            var langs = langsTask.Result ?? new();
            var mine = contributorsTask.Result?.FirstOrDefault(c =>
                string.Equals(c.Author?.Login, _username, StringComparison.OrdinalIgnoreCase));

            return new RepoStatsResult(
                langs,
                mine?.Total ?? 0,
                mine?.Weeks.Sum(w => w.Additions) ?? 0
            );
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