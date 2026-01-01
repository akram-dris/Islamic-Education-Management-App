using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Allocation : Entity
{
    public Guid TeacherId { get; set; }
    public Guid ClassId { get; set; }
    public Guid SubjectId { get; set; }

    public User? Teacher { get; set; }
    public Class? Class { get; set; }
    public Subject? Subject { get; set; }
}
