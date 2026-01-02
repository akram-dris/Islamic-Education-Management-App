using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.Update;
using QuranSchool.Domain.Entities;

namespace QuranSchool.UnitTests.Features.Users.Update;

public class UpdateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _handler = new UpdateUserCommandHandler(_userRepositoryMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "New Name", null);
        _userRepositoryMock.GetByIdAsync(command.UserId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.NotFound");
    }
}
