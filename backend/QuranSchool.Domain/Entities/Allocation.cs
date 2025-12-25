namespace QuranSchool.Domain.Entities;

public class Allocation
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public Guid ClassId { get; set; }
    public Guid SubjectId { get; set; }

    public User? Teacher { get; set; }
    public Class? Class { get; set; }
    public Subject? Subject { get; set; }
}
