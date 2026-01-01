using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Attendance.Delete;

public record DeleteAttendanceSessionCommand(Guid SessionId) : IRequest<Result>;
