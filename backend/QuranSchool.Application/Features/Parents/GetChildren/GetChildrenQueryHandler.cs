using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Parents.GetChildren;

public sealed class GetChildrenQueryHandler : IRequestHandler<GetChildrenQuery, Result<List<UserResponse>>>
{
    private readonly IUserRepository _userRepository;

    public GetChildrenQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserResponse>>> Handle(GetChildrenQuery request, CancellationToken cancellationToken)
    {
        var children = await _userRepository.GetByParentIdAsync(request.ParentId, cancellationToken);

        var response = children.Select(c => new UserResponse(
            c.Id,
            c.Username,
            c.FullName,
            c.Role.ToString()
        )).ToList();

        return response;
    }
}
