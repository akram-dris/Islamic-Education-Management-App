using MediatR;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Submissions.Delete;

public sealed class DeleteSubmissionCommandHandler : IRequestHandler<DeleteSubmissionCommand, Result>
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public DeleteSubmissionCommandHandler(
        ISubmissionRepository submissionRepository, 
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _submissionRepository = submissionRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(DeleteSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return Result.Failure(DomainErrors.Submission.NotFound);
        }

        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(DomainErrors.User.NotFound);
        }

        if (user.Role == UserRole.Student && submission.StudentId != user.Id)
        {
            return Result.Failure(DomainErrors.User.NotAuthorized);
        }

        // Teachers/Admins can delete any (or we could restrict Teacher to their class, but simpler for now).

        await _submissionRepository.DeleteAsync(submission, cancellationToken);

        return Result.Success();
    }
}
