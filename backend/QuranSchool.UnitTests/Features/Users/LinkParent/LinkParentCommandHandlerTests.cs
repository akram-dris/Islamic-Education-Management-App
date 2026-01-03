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
    public async Task Handle_ShouldReturnFailure_WhenParentNotFound()
    {
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns((User?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.ParentNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenStudentNotFound()
    {
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        var parent = User.Create("p", "p", "P", UserRole.Parent).Value;
        
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(parent);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns((User?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.StudentNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAlreadyLinked()
    {
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        var parent = User.Create("p", "p", "P", UserRole.Parent).Value;
        var student = User.Create("s", "s", "S", UserRole.Student).Value;
        
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(parent);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns(student);
        _userRepositoryMock.IsParentLinkedAsync(command.ParentId, command.StudentId).Returns(true);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.AlreadyLinked);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new LinkParentCommand(Guid.NewGuid(), Guid.NewGuid());
        var parent = User.Create("p", "p", "P", UserRole.Parent).Value;
        var student = User.Create("s", "s", "S", UserRole.Student).Value;
        
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(parent);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns(student);
        _userRepositoryMock.IsParentLinkedAsync(command.ParentId, command.StudentId).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.Received(1).AddParentStudentLinkAsync(Arg.Any<ParentStudent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDomainLinkCreationFails()
    {
        var command = new LinkParentCommand(Guid.Empty, Guid.NewGuid());
        var parent = User.Create("p", "p", "P", UserRole.Parent).Value;
        var student = User.Create("s", "s", "S", UserRole.Student).Value;
        
        _userRepositoryMock.GetByIdAsync(command.ParentId).Returns(parent);
        _userRepositoryMock.GetByIdAsync(command.StudentId).Returns(student);
        _userRepositoryMock.IsParentLinkedAsync(command.ParentId, command.StudentId).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyParentId);
    }
}