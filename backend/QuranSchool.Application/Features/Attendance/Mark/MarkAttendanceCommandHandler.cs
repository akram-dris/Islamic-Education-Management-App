using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Attendance.Mark;

public sealed class MarkAttendanceCommandHandler : IRequestHandler<MarkAttendanceCommand, Result>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IAllocationRepository _allocationRepository;
    private readonly IUserContext _userContext;

    public MarkAttendanceCommandHandler(
        IAttendanceRepository attendanceRepository,
        IAllocationRepository allocationRepository,
        IUserContext userContext)
    {
        _attendanceRepository = attendanceRepository;
        _allocationRepository = allocationRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(MarkAttendanceCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _allocationRepository.GetByIdAsync(request.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result.Failure(DomainErrors.Allocation.NotFound);
        }

        if (allocation.TeacherId != _userContext.UserId)
        {
            return Result.Failure(DomainErrors.User.NotAuthorized);
        }

        var session = await _attendanceRepository.GetSessionByAllocationAndDateAsync(request.AllocationId, request.Date, cancellationToken);
        if (session is null)
        {
            session = new AttendanceSession
            {
                Id = Guid.NewGuid(),
                AllocationId = request.AllocationId,
                SessionDate = request.Date
            };
            await _attendanceRepository.AddSessionAsync(session, cancellationToken);
        }

        foreach (var recordDto in request.Records)
        {
            var existingRecord = await _attendanceRepository.GetRecordBySessionAndStudentAsync(session.Id, recordDto.StudentId, cancellationToken);
            if (existingRecord is null)
            {
                var record = new AttendanceRecord
                {
                    Id = Guid.NewGuid(),
                    AttendanceSessionId = session.Id,
                    StudentId = recordDto.StudentId,
                    Status = recordDto.Status
                };
                await _attendanceRepository.AddRecordAsync(record, cancellationToken);
            }
            else
            {
                existingRecord.Status = recordDto.Status;
                await _attendanceRepository.UpdateRecordAsync(existingRecord, cancellationToken);
            }
        }

        return Result.Success();
    }
}
