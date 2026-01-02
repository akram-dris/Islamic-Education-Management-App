using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Attendance.Update;

public sealed class UpdateAttendanceSessionCommandHandler : IRequestHandler<UpdateAttendanceSessionCommand, Result>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IAllocationRepository _allocationRepository;
    private readonly IUserContext _userContext;

    public UpdateAttendanceSessionCommandHandler(
        IAttendanceRepository attendanceRepository,
        IAllocationRepository allocationRepository,
        IUserContext userContext)
    {
        _attendanceRepository = attendanceRepository;
        _allocationRepository = allocationRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateAttendanceSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _attendanceRepository.GetSessionByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            return Result.Failure(Error.NotFound("Attendance.SessionNotFound", "Attendance session not found."));
        }

        var allocation = await _allocationRepository.GetByIdAsync(session.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result.Failure(DomainErrors.Allocation.NotFound);
        }

        if (allocation.TeacherId != _userContext.UserId)
        {
             return Result.Failure(DomainErrors.User.NotAuthorized);
        }

        // Update Date
        if (session.SessionDate != request.Date)
        {
             var existingSession = await _attendanceRepository.GetSessionByAllocationAndDateAsync(session.AllocationId, request.Date, cancellationToken);
             if (existingSession != null && existingSession.Id != session.Id)
             {
                 return Result.Failure(Error.Conflict("Attendance.DuplicateSession", "A session already exists for this date."));
             }
             
             var updateResult = session.Update(request.Date);
             if (updateResult.IsFailure)
             {
                 return updateResult;
             }

             await _attendanceRepository.UpdateSessionAsync(session, cancellationToken);
        }

        // Update Records
        foreach (var recordDto in request.Records)
        {
            var existingRecord = await _attendanceRepository.GetRecordBySessionAndStudentAsync(session.Id, recordDto.StudentId, cancellationToken);
            if (existingRecord is null)
            {
                var recordResult = AttendanceRecord.Create(session.Id, recordDto.StudentId, recordDto.Status);
                if (recordResult.IsFailure)
                {
                    return Result.Failure(recordResult.Error);
                }
                
                await _attendanceRepository.AddRecordAsync(recordResult.Value, cancellationToken);
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