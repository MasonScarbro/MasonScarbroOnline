namespace MasonScarbroOnline.Models
{
    public class PageView
    {
        public int Id { get; set; }
        public string Path { get; set; }

        public string? Referrer { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
