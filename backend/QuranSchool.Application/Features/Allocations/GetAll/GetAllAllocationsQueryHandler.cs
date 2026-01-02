using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Allocations.GetAll;

public sealed class GetAllAllocationsQueryHandler : IRequestHandler<GetAllAllocationsQuery, Result<List<AllocationResponse>>>
{
    private readonly IAllocationRepository _allocationRepository;

    public GetAllAllocationsQueryHandler(IAllocationRepository allocationRepository)
    {
        _allocationRepository = allocationRepository;
    }

    public async Task<Result<List<AllocationResponse>>> Handle(GetAllAllocationsQuery request, CancellationToken cancellationToken)
    {
        var allocations = await _allocationRepository.GetAllAsync(cancellationToken);

        var response = allocations.Select(a => new AllocationResponse(
            a.Id,
            a.TeacherId,
            a.Teacher?.FullName ?? "N/A",
            a.ClassId,
            a.Class?.Name ?? "N/A",
            a.SubjectId,
            a.Subject?.Name ?? "N/A")).ToList();

        return response;
    }
}
