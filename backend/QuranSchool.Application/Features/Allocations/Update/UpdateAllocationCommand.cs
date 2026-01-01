using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Allocations.Update;

public record UpdateAllocationCommand(
    Guid AllocationId,
    Guid TeacherId,
    Guid ClassId,
    Guid SubjectId) : IRequest<Result>;
