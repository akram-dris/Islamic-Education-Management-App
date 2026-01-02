using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.Update;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Classes.Update;

public class UpdateClassCommandHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly UpdateClassCommandHandler _handler;

    public UpdateClassCommandHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new UpdateClassCommandHandler(
            _classRepositoryMock,
            _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClassNotFound()
    {
        // Arrange
        var command = new UpdateClassCommand(Guid.NewGuid(), "New Name");
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns((Class?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsDuplicate()
    {
        // Arrange
        var command = new UpdateClassCommand(Guid.NewGuid(), "Duplicate Name");
        var existingClass = Class.Create("Old Name").Value;
        
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns(existingClass);
        _classRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.DuplicateName);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var command = new UpdateClassCommand(Guid.NewGuid(), "New Name");
        var existingClass = Class.Create("Old Name").Value;
        
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns(existingClass);
        _classRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _classRepositoryMock.Received(1).UpdateAsync(Arg.Is<Class>(c => c.Name == command.Name), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsEmpty()
    {
         // Arrange
        var command = new UpdateClassCommand(Guid.NewGuid(), "");
        var existingClass = Class.Create("Old Name").Value;
        
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns(existingClass);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.EmptyName);
    }
}
