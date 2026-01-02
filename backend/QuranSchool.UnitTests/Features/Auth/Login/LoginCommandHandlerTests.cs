using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Auth.Login;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IJwtProvider _jwtProviderMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _jwtProviderMock = Substitute.For<IJwtProvider>();
        _handler = new LoginCommandHandler(_userRepositoryMock, _passwordHasherMock, _jwtProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var command = new LoginCommand("nonexistent", "password");
        _userRepositoryMock.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var command = new LoginCommand("user", "wrong_password");
        var user = User.Create("user", "correct_hash", "User", UserRole.Student).Value;
        _userRepositoryMock.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasherMock.Verify(command.Password, user.PasswordHash).Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreCorrect()
    {
        // Arrange
        var command = new LoginCommand("user", "correct_password");
        var user = User.Create("user", "hashed_password", "User", UserRole.Student).Value;
        
        _userRepositoryMock.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasherMock.Verify(command.Password, user.PasswordHash).Returns(true);
        _jwtProviderMock.Generate(user).Returns("valid_token");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().Be("valid_token");
    }
}
