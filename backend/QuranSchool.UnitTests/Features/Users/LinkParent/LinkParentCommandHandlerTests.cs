using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Users.LinkParent;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Users.LinkParent;

public class LinkParentCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly LinkParentCommandHandler _handler;

    public LinkParentCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _handler = new LinkParentCommandHandler(_userRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenStudentNotFound()
    {
        // Arrange
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(User.Create("d", "d", "d", UserRole.Parent).Value);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns((User?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.StudentNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAlreadyLinked()
    {
        // Arrange
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(User.Create("d", "d", "d", UserRole.Parent).Value);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns(User.Create("d", "d", "d", UserRole.Student).Value);
        _userRepositoryMock.IsParentLinkedAsync(command.ParentId, command.StudentId).Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.AlreadyLinked);
    }
}
