namespace QuranSchool.Domain.Entities;

public class Submission
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; }

    public Assignment? Assignment { get; set; }
    public User? Student { get; set; }
}
