using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Assignments.Create;

public record CreateAssignmentCommand(
    Guid AllocationId,
    string Title,
    string? Description,
    DateOnly DueDate) : IRequest<Result<Guid>>;
