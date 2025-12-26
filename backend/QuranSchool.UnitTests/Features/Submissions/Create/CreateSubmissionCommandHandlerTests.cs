using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Submissions.Create;
using QuranSchool.Domain.Entities;

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
    public async Task Handle_ShouldReturnFailure_WhenAlreadySubmitted()
    {
        // Arrange
        var command = new CreateSubmissionCommand(Guid.NewGuid(), Guid.NewGuid(), "url");
        _assignmentRepositoryMock.GetByIdAsync(command.AssignmentId).Returns(new Assignment());
        _submissionRepositoryMock.ExistsAsync(command.StudentId, command.AssignmentId).Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Submission.AlreadySubmitted");
    }
}
