using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Submissions.Delete;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Submissions.Delete;

public class DeleteSubmissionCommandHandlerTests
{
    private readonly ISubmissionRepository _submissionRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUserContext _userContextMock;
    private readonly DeleteSubmissionCommandHandler _handler;

    public DeleteSubmissionCommandHandlerTests()
    {
        _submissionRepositoryMock = Substitute.For<ISubmissionRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new DeleteSubmissionCommandHandler(_submissionRepositoryMock, _userRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSubmissionNotFound()
    {
        var command = new DeleteSubmissionCommand(Guid.NewGuid());
        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId, Arg.Any<CancellationToken>())
            .Returns((Submission?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
    {
        var command = new DeleteSubmissionCommand(Guid.NewGuid());
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var userId = Guid.NewGuid();

        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId, Arg.Any<CancellationToken>())
            .Returns(submission);
        _userContextMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotAuthorized()
    {
        var command = new DeleteSubmissionCommand(Guid.NewGuid());
        var studentId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var submission = Submission.Create(Guid.NewGuid(), studentId, "url").Value;
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
        
        // Reflection to set ID for test if needed, or assume repository returns object with correct ID
        // Mocking repo to return specific user
        
        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId, Arg.Any<CancellationToken>())
            .Returns(submission);
        _userContextMock.UserId.Returns(otherId);
        _userRepositoryMock.GetByIdAsync(otherId, Arg.Any<CancellationToken>())
            .Returns(user); 
            // The user ID matches the mock return, but we need to ensure user.Id != submission.StudentId
            // The User.Create generates a new ID.
            // submission.StudentId is studentId.
            // user.Id will be random.

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSubmissionExistsAndAuthorized()
    {
        var command = new DeleteSubmissionCommand(Guid.NewGuid());
        var studentId = Guid.NewGuid();
        var submission = Submission.Create(Guid.NewGuid(), studentId, "url").Value;
        
        // We need to ensure the user returned by repository has ID = studentId
        // But User.Id is init-only and generated in constructor.
        // We can't easily force it without reflection or a proper constructor/factory that accepts ID.
        // Alternatively, we use the user's ID for the submission.
        
        var user = User.Create("u", "p", "n", UserRole.Student).Value;
        var submissionForUser = Submission.Create(Guid.NewGuid(), user.Id, "url").Value;

        _submissionRepositoryMock.GetByIdAsync(command.SubmissionId, Arg.Any<CancellationToken>())
            .Returns(submissionForUser);
        _userContextMock.UserId.Returns(user.Id);
        _userRepositoryMock.GetByIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _submissionRepositoryMock.Received(1).DeleteAsync(submissionForUser, Arg.Any<CancellationToken>());
    }
}