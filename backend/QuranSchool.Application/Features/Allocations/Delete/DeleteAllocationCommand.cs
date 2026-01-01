using MediatR;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Allocations.Delete;

public record DeleteAllocationCommand(Guid AllocationId) : IRequest<Result>;
