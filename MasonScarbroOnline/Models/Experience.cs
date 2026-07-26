namespace MasonScarbroOnline.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string DateRange => $"{StartDate:MMM yyyy} - {(EndDate == null ? "Present" : $"{EndDate:MMM yyyy}")}";
        public IEnumerable<string> DescriptionLines =>
        Description?
            .Split(['*', '•'], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
        ?? Enumerable.Empty<string>();

        
    }
}
