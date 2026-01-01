using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class AttendanceSession : Entity
{
    private AttendanceSession(Guid allocationId, DateOnly sessionDate)
    {
        AllocationId = allocationId;
        SessionDate = sessionDate;
    }

    private AttendanceSession()
    {
    }

    public Guid AllocationId { get; private set; }
    public DateOnly SessionDate { get; private set; }

    public Allocation? Allocation { get; set; }

    public static Result<AttendanceSession> Create(Guid allocationId, DateOnly sessionDate)
    {
        if (allocationId == Guid.Empty)
        {
            return Result<AttendanceSession>.Failure(DomainErrors.AttendanceSession.EmptyAllocationId);
        }

        return new AttendanceSession(allocationId, sessionDate);
    }
}
