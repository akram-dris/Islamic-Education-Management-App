using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Assignments.Delete;

public record DeleteAssignmentCommand(Guid AssignmentId) : IRequest<Result>;
