using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Assignments.Update;

public record UpdateAssignmentCommand(
    Guid AssignmentId,
    string Title,
    string? Description,
    DateOnly DueDate) : IRequest<Result>;
