using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Enrollments.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Enrollments.Delete;

public class DeleteEnrollmentCommandHandlerTests
{
    private readonly IEnrollmentRepository _enrollmentRepositoryMock;
    private readonly DeleteEnrollmentCommandHandler _handler;

    public DeleteEnrollmentCommandHandlerTests()
    {
        _enrollmentRepositoryMock = Substitute.For<IEnrollmentRepository>();
        _handler = new DeleteEnrollmentCommandHandler(_enrollmentRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEnrollmentNotFound()
    {
        var command = new DeleteEnrollmentCommand(Guid.NewGuid());
        _enrollmentRepositoryMock.GetByIdAsync(command.EnrollmentId, Arg.Any<CancellationToken>())
            .Returns((Enrollment?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Enrollment.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEnrollmentExists()
    {
        var command = new DeleteEnrollmentCommand(Guid.NewGuid());
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _enrollmentRepositoryMock.GetByIdAsync(command.EnrollmentId, Arg.Any<CancellationToken>())
            .Returns(enrollment);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _enrollmentRepositoryMock.Received(1).DeleteAsync(enrollment, Arg.Any<CancellationToken>());
    }
}
