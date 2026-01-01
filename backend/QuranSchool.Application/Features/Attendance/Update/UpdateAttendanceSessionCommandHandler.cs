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
            // We need Session.NotFound, but it might not exist.
            // Using Allocation.NotFound as placeholder or creating new one?
            // Let's check DomainErrors again.
            // Using Allocation.NotFound is misleading.
            // I'll create Attendance.SessionNotFound?
            // For now, I'll use generic Error.NotFound if no specific one.
            return Result.Failure(Error.NotFound("Attendance.SessionNotFound", "Attendance session not found."));
        }

        var allocation = await _allocationRepository.GetByIdAsync(session.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result.Failure(DomainErrors.Allocation.NotFound);
        }

        // Auth check
        if (allocation.TeacherId != _userContext.UserId)
        {
             // Check if Admin? Assuming controller handles Role logic, but ownership check is stricter.
             // If Admin, this might fail if _userContext.UserId is Admin's ID.
             // But the command handler should ideally be agnostic of "Admin override" logic unless explicitly coded.
             // Or rely on DomainService/Policy.
             // For now, I'll allow it if Teacher OR ignore if Admin?
             // Actually, if _userContext.Role is Admin, we should skip.
             // But I don't have Role here easily.
             // I'll assume only Teacher updates their attendance, or Admin does it via different means?
             // Let's stick to Teacher check for now as per `MarkAttendance`.
             return Result.Failure(DomainErrors.User.NotAuthorized);
        }

        // Update Date
        if (session.SessionDate != request.Date)
        {
             // Check for duplicate session on new date
             var existingSession = await _attendanceRepository.GetSessionByAllocationAndDateAsync(session.AllocationId, request.Date, cancellationToken);
             if (existingSession != null && existingSession.Id != session.Id)
             {
                 return Result.Failure(Error.Conflict("Attendance.DuplicateSession", "A session already exists for this date."));
             }
             session.SessionDate = request.Date;
             await _attendanceRepository.UpdateSessionAsync(session, cancellationToken);
        }

        // Update Records
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
