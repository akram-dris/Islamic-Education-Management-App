using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Submissions.Grade;

public sealed class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IAllocationRepository _allocationRepository;
    private readonly IUserContext _userContext;

    public GradeSubmissionCommandHandler(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IAllocationRepository allocationRepository,
        IUserContext userContext)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _allocationRepository = allocationRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return Result.Failure(DomainErrors.Submission.NotFound);
        }

        var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken);
        var allocation = await _allocationRepository.GetByIdAsync(assignment!.AllocationId, cancellationToken);

        if (allocation!.TeacherId != _userContext.UserId)
        {
            return Result.Failure(DomainErrors.User.NotAuthorized);
        }

        submission.Grade = request.Grade;
        submission.Remarks = request.Remarks;

        await _submissionRepository.UpdateAsync(submission, cancellationToken);

        return Result.Success();
    }
}
