using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Assignments.GetMy;

public record AssignmentResponse(
    Guid Id,
    Guid AllocationId,
    string Title,
    string? Description,
    DateOnly DueDate,
    DateTime CreatedAt);

public record GetMyAssignmentsQuery(Guid StudentId) : IRequest<Result<List<AssignmentResponse>>>;
