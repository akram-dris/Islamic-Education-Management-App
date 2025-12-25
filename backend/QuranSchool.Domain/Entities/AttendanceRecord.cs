using QuranSchool.Domain.Enums;

namespace QuranSchool.Domain.Entities;

public class AttendanceRecord
{
    public Guid Id { get; set; }
    public Guid AttendanceSessionId { get; set; }
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }

    public AttendanceSession? AttendanceSession { get; set; }
    public User? Student { get; set; }
}
