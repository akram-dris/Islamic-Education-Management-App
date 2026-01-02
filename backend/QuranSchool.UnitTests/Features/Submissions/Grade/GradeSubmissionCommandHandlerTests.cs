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
    public async Task Handle_ShouldReturnFailure_WhenSubmissionNotFound()
    {
        var command = new GradeSubmissionCommand(Guid.NewGuid(), 90, "Good");
        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId).Returns((Submission?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotAuthorized()
    {
        var command = new GradeSubmissionCommand(Guid.NewGuid(), 90, "Good");
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId).Returns(submission);
        _assignmentRepositoryMock.GetByIdAsync(submission.AssignmentId).Returns(assignment);
        _allocationRepositoryMock.GetByIdAsync(assignment.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(Guid.NewGuid()); // Not the teacher

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenGradeIsInvalid()
    {
        var command = new GradeSubmissionCommand(Guid.NewGuid(), 110, "Invalid");
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;

        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId).Returns(submission);
        _assignmentRepositoryMock.GetByIdAsync(submission.AssignmentId).Returns(assignment);
        _allocationRepositoryMock.GetByIdAsync(assignment.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(allocation.TeacherId);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.InvalidGrade);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new GradeSubmissionCommand(Guid.NewGuid(), 95, "Excellent");
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;

        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId).Returns(submission);
        _assignmentRepositoryMock.GetByIdAsync(submission.AssignmentId).Returns(assignment);
        _allocationRepositoryMock.GetByIdAsync(assignment.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(allocation.TeacherId);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        submission.Grade.Should().Be(95);
        await _submissionRepositoryMock.Received(1).UpdateAsync(submission, Arg.Any<CancellationToken>());
    }
}
