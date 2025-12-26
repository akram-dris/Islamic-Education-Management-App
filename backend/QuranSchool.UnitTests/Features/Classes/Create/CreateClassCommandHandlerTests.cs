using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Domain.Entities;

namespace QuranSchool.UnitTests.Features.Classes.Create;

public class CreateClassCommandHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _cacheMock;
    private readonly CreateClassCommandHandler _handler;

    public CreateClassCommandHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _cacheMock = Substitute.For<IMemoryCache>();
        _handler = new CreateClassCommandHandler(_classRepositoryMock, _cacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClassNameIsNotUnique()
    {
        // Arrange
        var command = new CreateClassCommand("Class A");
        _classRepositoryMock.ExistsAsync(command.Name, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Class.DuplicateName");
    }
}
