using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Submissions.Grade;

public sealed class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly ISubmissionRepository _submissionRepository;

    public GradeSubmissionCommandHandler(ISubmissionRepository submissionRepository)
    {
        _submissionRepository = submissionRepository;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return Result.Failure(DomainErrors.Submission.NotFound);
        }

        submission.Grade = request.Grade;
        submission.Remarks = request.Remarks;

        await _submissionRepository.UpdateAsync(submission, cancellationToken);

        return Result.Success();
    }
}
