using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Enrollments.Create;

public record CreateEnrollmentCommand(Guid StudentId, Guid ClassId) : IRequest<Result<Guid>>;
