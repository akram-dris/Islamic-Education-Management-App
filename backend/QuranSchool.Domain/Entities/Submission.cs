using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Submission : Entity
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; }

    public Assignment? Assignment { get; set; }
    public User? Student { get; set; }
}
