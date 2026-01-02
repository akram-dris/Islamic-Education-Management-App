using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.Update;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

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
        var command = new UpdateUserCommand(Guid.NewGuid(), "Name", "Pass");
        _userRepositoryMock.GetByIdAsync(command.UserId).Returns((User?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "", "Pass"); // Empty name
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
        _userRepositoryMock.GetByIdAsync(command.UserId).Returns(user);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyFullName);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new UpdateUserCommand(Guid.NewGuid(), "New Name", "New Pass");
        var user = User.Create("u", "p", "Old Name", UserRole.Student).Value;
        _userRepositoryMock.GetByIdAsync(command.UserId).Returns(user);
        _passwordHasherMock.Hash(command.Password!).Returns("hashed");

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("New Name");
        user.PasswordHash.Should().Be("hashed");
        await _userRepositoryMock.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }
}