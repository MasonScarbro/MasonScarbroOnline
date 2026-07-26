namespace MasonScarbroOnline.Models
{
    public class Thought
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
