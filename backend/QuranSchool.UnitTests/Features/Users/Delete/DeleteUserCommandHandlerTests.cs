using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Users.Delete;

public class DeleteUserCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_userRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());
        _userRepositoryMock.GetByIdAsync(command.UserId, default).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
        _userRepositoryMock.GetByIdAsync(command.UserId, default).Returns(user);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.Received(1).UpdateAsync(user, default);
        user.IsDeleted.Should().BeTrue();
    }
}
