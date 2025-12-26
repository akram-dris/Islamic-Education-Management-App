using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Assignments.Create;
using QuranSchool.Domain.Entities;

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
        result.Error.Code.Should().Be("Allocation.NotFound");
    }
}
