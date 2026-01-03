using MediatR;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Attendance.Mark;

public record AttendanceRecordDto(Guid StudentId, AttendanceStatus Status);

public record MarkAttendanceCommand(
    Guid AllocationId, 
    DateOnly Date, 
    List<AttendanceRecordDto> Records) : IRequest<Result<Guid>>;
