using System.ComponentModel.DataAnnotations;

namespace MasonScarbroOnline.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Description = "Leave blank if this is your current role.")]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        public string DateRange => $"{StartDate:MMM yyyy} - {(EndDate == null ? "Present" : $"{EndDate:MMM yyyy}")}";
        public IEnumerable<string> DescriptionLines =>
            Description?.Split(['*', '•'], StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
            ?? Enumerable.Empty<string>();

    }
}
