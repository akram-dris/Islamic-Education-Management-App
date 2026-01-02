using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Parents.GetChildren;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Parents.GetChildren;

public class GetChildrenQueryHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly GetChildrenQueryHandler _handler;

    public GetChildrenQueryHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _handler = new GetChildrenQueryHandler(_userRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnChildren_ForCurrentParent()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        var child = User.Create("child", "hash", "Child Name", UserRole.Student).Value;
        _userRepositoryMock.GetByParentIdAsync(parentId, default).Returns(new List<User> { child });

        // Act
        var result = await _handler.Handle(new GetChildrenQuery(parentId), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].FullName.Should().Be("Child Name");
    }
}
