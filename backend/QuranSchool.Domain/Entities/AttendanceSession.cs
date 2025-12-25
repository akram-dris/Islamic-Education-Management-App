namespace QuranSchool.Domain.Entities;

public class AttendanceSession
{
    public Guid Id { get; set; }
    public Guid AllocationId { get; set; }
    public DateOnly SessionDate { get; set; }

    public Allocation? Allocation { get; set; }
}
