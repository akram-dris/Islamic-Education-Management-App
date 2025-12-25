namespace QuranSchool.Domain.Entities;

public class Enrollment
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }

    public User? Student { get; set; }
    public Class? Class { get; set; }
}
