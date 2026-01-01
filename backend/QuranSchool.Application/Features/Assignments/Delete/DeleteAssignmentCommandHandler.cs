using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Assignments.Delete;

public sealed class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand, Result>
{
    private readonly IAssignmentRepository _assignmentRepository;

    public DeleteAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Result> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment is null)
        {
            return Result.Failure(DomainErrors.Assignment.NotFound);
        }

        await _assignmentRepository.DeleteAsync(assignment, cancellationToken);

        return Result.Success();
    }
}
