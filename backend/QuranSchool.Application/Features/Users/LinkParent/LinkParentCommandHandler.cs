using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

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
            return Result.Failure(Error.NotFound("User.ParentNotFound", "Parent not found."));
        }

        var student = await _userRepository.GetByIdAsync(request.StudentId, cancellationToken);
        if (student is null || student.Role != UserRole.Student)
        {
            return Result.Failure(Error.NotFound("User.StudentNotFound", "Student not found."));
        }

        if (await _userRepository.IsParentLinkedAsync(request.ParentId, request.StudentId, cancellationToken))
        {
            return Result.Failure(Error.Conflict("User.AlreadyLinked", "Student is already linked to this parent."));
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
