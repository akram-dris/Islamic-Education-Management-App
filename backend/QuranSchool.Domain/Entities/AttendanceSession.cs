using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class AttendanceSession : Entity
{
    public Guid AllocationId { get; set; }
    public DateOnly SessionDate { get; set; }

    public Allocation? Allocation { get; set; }
}
