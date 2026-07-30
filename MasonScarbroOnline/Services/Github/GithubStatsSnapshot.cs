namespace MasonScarbroOnline.Services.Github
{
    public class GitHubStatsSnapshot
    {
        public int TotalRepos { get; set; }
        public int TotalCommits { get; set; }
        public int LinesContributed { get; set; }
        public List<(string Language, double Percent)> TopLanguages { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
}
