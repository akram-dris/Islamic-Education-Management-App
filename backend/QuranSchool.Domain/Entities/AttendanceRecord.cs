using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class AttendanceRecord : Entity
{
    private AttendanceRecord(Guid attendanceSessionId, Guid studentId, AttendanceStatus status)
    {
        AttendanceSessionId = attendanceSessionId;
        StudentId = studentId;
        Status = status;
    }

    private AttendanceRecord()
    {
    }

    public Guid AttendanceSessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceStatus Status { get; set; }

    public AttendanceSession? AttendanceSession { get; set; }
    public User? Student { get; set; }

    public static Result<AttendanceRecord> Create(Guid attendanceSessionId, Guid studentId, AttendanceStatus status)
    {
        if (attendanceSessionId == Guid.Empty)
        {
            return Result<AttendanceRecord>.Failure(DomainErrors.AttendanceRecord.EmptySessionId);
        }

        if (studentId == Guid.Empty)
        {
            return Result<AttendanceRecord>.Failure(DomainErrors.AttendanceRecord.EmptyStudentId);
        }

        return new AttendanceRecord(attendanceSessionId, studentId, status);
    }
}