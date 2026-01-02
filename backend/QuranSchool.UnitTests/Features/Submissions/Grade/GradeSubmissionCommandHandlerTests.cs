using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Submissions.Grade;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Submissions.Grade;

public class GradeSubmissionCommandHandlerTests
{
    private readonly ISubmissionRepository _submissionRepositoryMock;
    private readonly IAssignmentRepository _assignmentRepositoryMock;
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly IUserContext _userContextMock;
    private readonly GradeSubmissionCommandHandler _handler;

    public GradeSubmissionCommandHandlerTests()
    {
        _submissionRepositoryMock = Substitute.For<ISubmissionRepository>();
        _assignmentRepositoryMock = Substitute.For<IAssignmentRepository>();
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GradeSubmissionCommandHandler(
            _submissionRepositoryMock,
            _assignmentRepositoryMock,
            _allocationRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTeacherNotAllocated()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var allocation = Allocation.Create(otherTeacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var assignment = Assignment.Create(allocation.Id, "Title", "Desc", DateOnly.FromDateTime(DateTime.Now)).Value;
        var submission = Submission.Create(assignment.Id, Guid.NewGuid(), "File").Value;

        _submissionRepositoryMock.GetByIdAsync(Arg.Any<Guid>()).Returns(submission);
        _assignmentRepositoryMock.GetByIdAsync(submission.AssignmentId).Returns(assignment);
        _allocationRepositoryMock.GetByIdAsync(assignment.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);

        var command = new GradeSubmissionCommand(Guid.NewGuid(), 10, "Great");

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }
}
