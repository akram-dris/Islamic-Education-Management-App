using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Assignments.Create;

public sealed class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, Result<Guid>>
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IAllocationRepository _allocationRepository;

    public CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository, IAllocationRepository allocationRepository)
    {
        _assignmentRepository = assignmentRepository;
        _allocationRepository = allocationRepository;
    }

    public async Task<Result<Guid>> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _allocationRepository.GetByIdAsync(request.AllocationId, cancellationToken);
        if (allocation is null)
        {
            return Result<Guid>.Failure(DomainErrors.Allocation.NotFound);
        }

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            AllocationId = request.AllocationId,
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow
        };

        await _assignmentRepository.AddAsync(assignment, cancellationToken);

        return assignment.Id;
    }
}
