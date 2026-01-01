using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Enrollment : Entity
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }

    public User? Student { get; set; }
    public Class? Class { get; set; }
}
