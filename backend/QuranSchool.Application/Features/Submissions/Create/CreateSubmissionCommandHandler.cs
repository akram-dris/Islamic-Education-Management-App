using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;

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
            return Result<Guid>.Failure(Error.NotFound("Assignment.NotFound", "Assignment not found."));
        }

        if (await _submissionRepository.ExistsAsync(request.StudentId, request.AssignmentId, cancellationToken))
        {
            return Result<Guid>.Failure(Error.Conflict("Submission.AlreadySubmitted", "You have already submitted this assignment."));
        }

        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            FileUrl = request.FileUrl,
            SubmittedAt = DateTime.UtcNow
        };

        await _submissionRepository.AddAsync(submission, cancellationToken);

        return submission.Id;
    }
}
