using MediatR;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Attendance.Update;

public record UpdateAttendanceSessionCommand(
    Guid SessionId,
    DateOnly Date,
    List<AttendanceRecordDto> Records) : IRequest<Result>;
