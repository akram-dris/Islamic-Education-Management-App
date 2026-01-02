using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
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
    public async Task Handle_ShouldReturnPagedUsers()
    {
        // Arrange
        var query = new GetUsersQuery(null, null, 1, 10);
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
        var pagedList = new PagedList<User>(new List<User> { user }, 1, 10, 1);
        
        _userRepositoryMock.GetPagedAsync(query.SearchTerm, query.Role, query.Page, query.PageSize, default)
            .Returns(pagedList);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
        result.Value.Items[0].Username.Should().Be("u");
    }
}
