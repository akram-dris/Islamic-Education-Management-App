using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Enrollments.Create;

namespace QuranSchool.UnitTests.Features.Enrollments.Create;

public class CreateEnrollmentCommandHandlerTests
{
    private readonly IEnrollmentRepository _enrollmentRepositoryMock;
    private readonly CreateEnrollmentCommandHandler _handler;

    public CreateEnrollmentCommandHandlerTests()
    {
        _enrollmentRepositoryMock = Substitute.For<IEnrollmentRepository>();
        _handler = new CreateEnrollmentCommandHandler(_enrollmentRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEnrollmentAlreadyExists()
    {
        // Arrange
        var command = new CreateEnrollmentCommand(Guid.NewGuid(), Guid.NewGuid());
        _enrollmentRepositoryMock.ExistsAsync(command.StudentId, command.ClassId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Enrollment.Duplicate");
    }
}
