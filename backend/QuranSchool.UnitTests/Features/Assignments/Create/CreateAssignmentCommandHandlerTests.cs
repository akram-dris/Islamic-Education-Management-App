using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Assignments.Create;

public class CreateAssignmentCommandHandlerTests
{
    private readonly IAssignmentRepository _assignmentRepositoryMock;
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly CreateAssignmentCommandHandler _handler;

    public CreateAssignmentCommandHandlerTests()
    {
        _assignmentRepositoryMock = Substitute.For<IAssignmentRepository>();
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _handler = new CreateAssignmentCommandHandler(_assignmentRepositoryMock, _allocationRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        // Arrange
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "Title", "Desc", DateOnly.FromDateTime(DateTime.Now));
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns((Allocation?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.NotFound);
    }
    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAssignmentCreationFails()
    {
        // Arrange - Title is valid, but let's send an Empty Allocation ID if validation was checked before repo get. 
        // But repo get is first.
        // Assignment.Create checks for empty allocationId and empty title.
        // If we send valid AllocationId but empty Title, Create will fail.
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "", "Desc", DateOnly.FromDateTime(DateTime.Now));
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.EmptyTitle);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAssignmentIsValid()
    {
        // Arrange
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "Title", "Desc", DateOnly.FromDateTime(DateTime.Now));
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        await _assignmentRepositoryMock.Received(1).AddAsync(Arg.Any<Assignment>(), Arg.Any<CancellationToken>());
    }
}
