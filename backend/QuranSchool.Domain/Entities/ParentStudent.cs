namespace QuranSchool.Domain.Entities;

public class ParentStudent
{
    public Guid ParentId { get; set; }
    public Guid StudentId { get; set; }

    public User? Parent { get; set; }
    public User? Student { get; set; }
}
