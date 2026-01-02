using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Assignments.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Assignments.Delete;

public class DeleteAssignmentCommandHandlerTests
{
    private readonly IAssignmentRepository _assignmentRepositoryMock;
    private readonly DeleteAssignmentCommandHandler _handler;

    public DeleteAssignmentCommandHandlerTests()
    {
        _assignmentRepositoryMock = Substitute.For<IAssignmentRepository>();
        _handler = new DeleteAssignmentCommandHandler(_assignmentRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAssignmentNotFound()
    {
        // Arrange
        var command = new DeleteAssignmentCommand(Guid.NewGuid());
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId, Arg.Any<CancellationToken>())
            .Returns((Assignment?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.NotFound);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAssignmentExists()
    {
        // Arrange
        var command = new DeleteAssignmentCommand(Guid.NewGuid());
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var assignment = Assignment.Create(allocation.Id, "T", "D", DateOnly.FromDateTime(DateTime.Now)).Value;
        
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId, Arg.Any<CancellationToken>())
            .Returns(assignment);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _assignmentRepositoryMock.Received(1).DeleteAsync(assignment, Arg.Any<CancellationToken>());
    }
}