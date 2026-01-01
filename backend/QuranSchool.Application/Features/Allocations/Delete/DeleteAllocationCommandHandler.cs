using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Allocations.Delete;

public sealed class DeleteAllocationCommandHandler : IRequestHandler<DeleteAllocationCommand, Result>
{
    private readonly IAllocationRepository _allocationRepository;

    public DeleteAllocationCommandHandler(IAllocationRepository allocationRepository)
    {
        _allocationRepository = allocationRepository;
    }

    public async Task<Result> Handle(DeleteAllocationCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _allocationRepository.GetByIdAsync(request.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result.Failure(DomainErrors.Allocation.NotFound);
        }

        await _allocationRepository.DeleteAsync(allocation, cancellationToken);

        return Result.Success();
    }
}
