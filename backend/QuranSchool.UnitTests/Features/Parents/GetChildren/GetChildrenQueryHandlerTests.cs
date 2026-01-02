using FluentAssertions;
using NSubstitute;
using Xunit;
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
    public async Task Handle_ShouldReturnChildrenForParent()
    {
        var parentId = Guid.NewGuid();
        var query = new GetChildrenQuery(parentId);
        var children = new List<User> { User.Create("c", "p", "C", UserRole.Student).Value };
        
        _userRepositoryMock.GetByParentIdAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(children);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}