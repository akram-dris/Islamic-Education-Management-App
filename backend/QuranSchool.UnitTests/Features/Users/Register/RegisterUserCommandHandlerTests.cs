using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Users.Register;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _handler = new RegisterUserCommandHandler(_userRepositoryMock, _passwordHasherMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUsernameIsNotUnique()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "password", "Test User", UserRole.Student);
        
        _userRepositoryMock.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns(new User());

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.DuplicateUsername);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsRegistered()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "password", "Test User", UserRole.Student);
        
        _userRepositoryMock.GetByUsernameAsync(command.Username, Arg.Any<CancellationToken>())
            .Returns((User?)null);
        
        _passwordHasherMock.Hash(command.Password).Returns("hashed_password");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
