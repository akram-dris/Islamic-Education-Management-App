using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Submissions.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Submissions.Create;

public class CreateSubmissionCommandHandlerTests
{
    private readonly ISubmissionRepository _submissionRepositoryMock;
    private readonly IAssignmentRepository _assignmentRepositoryMock;
    private readonly CreateSubmissionCommandHandler _handler;

    public CreateSubmissionCommandHandlerTests()
    {
        _submissionRepositoryMock = Substitute.For<ISubmissionRepository>();
        _assignmentRepositoryMock = Substitute.For<IAssignmentRepository>();
        _handler = new CreateSubmissionCommandHandler(_submissionRepositoryMock, _assignmentRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAssignmentNotFound()
    {
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), "url");
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns((Assignment?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenDueDatePassed()
    {
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), "url");
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))).Value;
        
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns(assignment);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.PastDueDate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAlreadySubmitted()
    {
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), "url");
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))).Value;
        
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns(assignment);
        _submissionRepositoryMock.ExistsAsync(command.StudentId, command.AssignmentId).Returns(true);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.AlreadySubmitted);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubmissionCreateFails()
    {
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), ""); // Empty URL
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))).Value;
        
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns(assignment);
        _submissionRepositoryMock.ExistsAsync(command.StudentId, command.AssignmentId).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.EmptyFileUrl);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), "url");
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))).Value;
        
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns(assignment);
        _submissionRepositoryMock.ExistsAsync(command.StudentId, command.AssignmentId).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _submissionRepositoryMock.Received(1).AddAsync(Arg.Any<Submission>(), Arg.Any<CancellationToken>());
    }
}