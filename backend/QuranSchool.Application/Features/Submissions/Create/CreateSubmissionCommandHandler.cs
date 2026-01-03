using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Submissions.Create;

public sealed class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Result<Guid>>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;

    public CreateSubmissionCommandHandler(ISubmissionRepository submissionRepository, IAssignmentRepository assignmentRepository)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Result<Guid>> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment is null)
        {
            return Result<Guid>.Failure(DomainErrors.Assignment.NotFound);
        }

        if (assignment.DueDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return Result<Guid>.Failure(DomainErrors.Assignment.PastDueDate);
        }

        if (await _submissionRepository.ExistsAsync(request.StudentId, request.AssignmentId, cancellationToken))
        {
            return Result<Guid>.Failure(DomainErrors.Submission.AlreadySubmitted);
        }

        var submissionResult = Submission.Create(
            request.AssignmentId,
            request.StudentId,
            request.FileUrl);

        if (submissionResult.IsFailure)
        {
            return Result<Guid>.Failure(submissionResult.Error);
        }

        var submission = submissionResult.Value;

        await _submissionRepository.AddAsync(submission, cancellationToken);

        return submission.Id;
    }
}
