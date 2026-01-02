using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Allocations.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Allocations.Delete;

public class DeleteAllocationCommandHandlerTests
{
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly DeleteAllocationCommandHandler _handler;

    public DeleteAllocationCommandHandlerTests()
    {
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _handler = new DeleteAllocationCommandHandler(_allocationRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        // Arrange
        var command = new DeleteAllocationCommand(Guid.NewGuid());
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns((Allocation?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllocationExists()
    {
        // Arrange
        var command = new DeleteAllocationCommand(Guid.NewGuid());
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _allocationRepositoryMock.Received(1).DeleteAsync(allocation, Arg.Any<CancellationToken>());
    }
}
