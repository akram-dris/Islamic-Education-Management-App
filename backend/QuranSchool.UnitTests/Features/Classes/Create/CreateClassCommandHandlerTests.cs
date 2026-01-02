using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Classes.Create;

public class CreateClassCommandHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly CreateClassCommandHandler _handler;

    public CreateClassCommandHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new CreateClassCommandHandler(_classRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNameIsDuplicate()
    {
        var command = new CreateClassCommand("Duplicate");
        _classRepositoryMock.ExistsAsync(command.Name).Returns(true);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.DuplicateName);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClassCreateFails()
    {
        var command = new CreateClassCommand(""); // Empty name
        _classRepositoryMock.ExistsAsync(command.Name).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.EmptyName);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new CreateClassCommand("New Class");
        _classRepositoryMock.ExistsAsync(command.Name).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _classRepositoryMock.Received(1).AddAsync(Arg.Any<Class>(), Arg.Any<CancellationToken>());
    }
}