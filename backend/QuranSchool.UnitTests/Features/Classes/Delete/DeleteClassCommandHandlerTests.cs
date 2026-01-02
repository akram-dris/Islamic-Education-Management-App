using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Classes.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace QuranSchool.UnitTests.Features.Classes.Delete;

public class DeleteClassCommandHandlerTests
{
    private readonly IClassRepository _classRepositoryMock;
    private readonly IMemoryCache _memoryCacheMock;
    private readonly DeleteClassCommandHandler _handler;

    public DeleteClassCommandHandlerTests()
    {
        _classRepositoryMock = Substitute.For<IClassRepository>();
        _memoryCacheMock = Substitute.For<IMemoryCache>();
        _handler = new DeleteClassCommandHandler(_classRepositoryMock, _memoryCacheMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClassNotFound()
    {
        var command = new DeleteClassCommand(Guid.NewGuid());
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns((Class?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenClassExists()
    {
        var command = new DeleteClassCommand(Guid.NewGuid());
        var @class = Class.Create("Name").Value;
        
        _classRepositoryMock.GetByIdAsync(command.ClassId, Arg.Any<CancellationToken>())
            .Returns(@class);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _classRepositoryMock.Received(1).DeleteAsync(@class, Arg.Any<CancellationToken>());
    }
}
