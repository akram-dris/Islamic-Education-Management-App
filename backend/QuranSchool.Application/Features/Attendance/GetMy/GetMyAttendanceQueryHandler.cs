using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Attendance.GetMy;

public sealed class GetMyAttendanceQueryHandler : IRequestHandler<GetMyAttendanceQuery, Result<List<AttendanceResponse>>>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public GetMyAttendanceQueryHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<Result<List<AttendanceResponse>>> Handle(GetMyAttendanceQuery request, CancellationToken cancellationToken)
    {
        var records = await _attendanceRepository.GetRecordsByStudentIdAsync(request.StudentId, cancellationToken);

        var response = records.Select(r => new AttendanceResponse(
            r.Id,
            r.AttendanceSession!.SessionDate,
            r.AttendanceSession.Allocation!.Class!.Name,
            r.AttendanceSession.Allocation!.Subject!.Name,
            r.Status)).ToList();

        return response;
    }
}
