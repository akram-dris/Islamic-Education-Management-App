using MediatR;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Features.Attendance.GetMy;

public record AttendanceResponse(
    Guid Id,
    DateOnly Date,
    string ClassName,
    string SubjectName,
    AttendanceStatus Status);

public record GetMyAttendanceQuery(Guid StudentId) : IRequest<Result<List<AttendanceResponse>>>;
