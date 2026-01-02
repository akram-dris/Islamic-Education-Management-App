using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Allocations.Create;

public record CreateAllocationCommand(
    Guid TeacherId,
    Guid ClassId,
    Guid SubjectId) : IRequest<Result<Guid>>;
