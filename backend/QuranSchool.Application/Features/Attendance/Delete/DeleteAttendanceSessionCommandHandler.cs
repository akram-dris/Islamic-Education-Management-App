using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Attendance.Delete;

public sealed class DeleteAttendanceSessionCommandHandler : IRequestHandler<DeleteAttendanceSessionCommand, Result>
{
    private readonly IAttendanceRepository _attendanceRepository;

    public DeleteAttendanceSessionCommandHandler(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<Result> Handle(DeleteAttendanceSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _attendanceRepository.GetSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
             return Result.Failure(Error.NotFound("Attendance.SessionNotFound", "Attendance session not found."));
        }

        await _attendanceRepository.DeleteSessionAsync(session, cancellationToken);

        return Result.Success();
    }
}
