using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Enrollments.Create;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

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
    public async Task Handle_ShouldReturnFailure_WhenAlreadyEnrolled()
    {
        var command = new CreateEnrollmentCommand(Guid.NewGuid(), Guid.NewGuid());
        
        _enrollmentRepositoryMock.ExistsAsync(command.StudentId, command.ClassId).Returns(true);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Enrollment.Duplicate);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenDataIsValid()
    {
        var command = new CreateEnrollmentCommand(Guid.NewGuid(), Guid.NewGuid());
        
        _enrollmentRepositoryMock.ExistsAsync(command.StudentId, command.ClassId).Returns(false);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _enrollmentRepositoryMock.Received(1).AddAsync(Arg.Any<Enrollment>(), Arg.Any<CancellationToken>());
    }
}
