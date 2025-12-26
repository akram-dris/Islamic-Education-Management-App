using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Application.Features.Users.LinkParent;

public sealed class LinkParentCommandHandler : IRequestHandler<LinkParentCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public LinkParentCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(LinkParentCommand request, CancellationToken cancellationToken)
    {
        var parent = await _userRepository.GetByIdAsync(request.ParentId, cancellationToken);
        if (parent is null || parent.Role != UserRole.Parent)
        {
            return Result.Failure(DomainErrors.User.ParentNotFound);
        }

        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null || student.Role != UserRole.Student)
        {
            return Result.Failure(DomainErrors.User.StudentNotFound);
        }

        if (await _userRepository.IsParentLinkedAsync(request.ParentId, request.StudentId, cancellationToken))
        {
            return Result.Failure(DomainErrors.User.AlreadyLinked);
        }

        var link = new ParentStudent
        {
            ParentId = request.ParentId,
            StudentId = request.StudentId
        };

        await _userRepository.AddParentStudentLinkAsync(link, cancellationToken);

        return Result.Success();
    }
}
