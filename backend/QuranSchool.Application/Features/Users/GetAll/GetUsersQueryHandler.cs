using MediatR;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Application.Features.Users.GetAll;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedList<UserResponse>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedList<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var pagedUsers = await _userRepository.GetPagedAsync(
            request.SearchTerm,
            request.Role,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = pagedUsers.Items.Select(u => new UserResponse(
            u.Id,
            u.Username,
            u.FullName,
            u.Role.ToString()
        )).ToList();

        return new PagedList<UserResponse>(
            items, 
            pagedUsers.Page, 
            pagedUsers.PageSize, 
            pagedUsers.TotalCount);
    }
}
