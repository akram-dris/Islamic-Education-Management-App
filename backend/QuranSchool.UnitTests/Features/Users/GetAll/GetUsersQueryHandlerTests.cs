using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Users.GetAll;

public class GetUsersQueryHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _handler = new GetUsersQueryHandler(_userRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedList()
    {
        var query = new GetUsersQuery(Page: 1, PageSize: 10);
        var users = new List<User> { User.Create("u", "p", "f", UserRole.Student).Value };
        var pagedList = new PagedList<User>(users, 1, 1, 10);
        
        _userRepositoryMock.GetPagedAsync(query.SearchTerm, query.Role, query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(pagedList);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
    }
}
