using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Enrollments.Delete;

public record DeleteEnrollmentCommand(Guid EnrollmentId) : IRequest<Result>;
