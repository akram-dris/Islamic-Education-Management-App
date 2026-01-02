using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.Delete;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Attendance.Delete;

public class DeleteAttendanceSessionCommandHandlerTests
{
    private readonly IAttendanceRepository _attendanceRepositoryMock;
    private readonly DeleteAttendanceSessionCommandHandler _handler;

    public DeleteAttendanceSessionCommandHandlerTests()
    {
        _attendanceRepositoryMock = Substitute.For<IAttendanceRepository>();
        _handler = new DeleteAttendanceSessionCommandHandler(_attendanceRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSessionNotFound()
    {
        var command = new DeleteAttendanceSessionCommand(Guid.NewGuid());
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns((AttendanceSession?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Attendance.SessionNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenSessionExists()
    {
        var command = new DeleteAttendanceSessionCommand(Guid.NewGuid());
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns(session);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _attendanceRepositoryMock.Received(1).DeleteSessionAsync(session, Arg.Any<CancellationToken>());
    }
}