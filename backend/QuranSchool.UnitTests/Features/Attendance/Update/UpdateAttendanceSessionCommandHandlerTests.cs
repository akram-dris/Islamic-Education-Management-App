using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.Update;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Attendance.Update;

public class UpdateAttendanceSessionCommandHandlerTests
{
    private readonly IAttendanceRepository _attendanceRepositoryMock;
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly IUserContext _userContextMock;
    private readonly UpdateAttendanceSessionCommandHandler _handler;

    public UpdateAttendanceSessionCommandHandlerTests()
    {
        _attendanceRepositoryMock = Substitute.For<IAttendanceRepository>();
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new UpdateAttendanceSessionCommandHandler(
            _attendanceRepositoryMock,
            _allocationRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSessionNotFound()
    {
        // Arrange
        var command = new UpdateAttendanceSessionCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns((AttendanceSession?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        // Checked logic: returns Error.NotFound("Attendance.SessionNotFound", ...)
        result.Error.Code.Should().Be("Attendance.SessionNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        // Arrange
        var command = new UpdateAttendanceSessionCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var session = AttendanceSession.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now)).Value;
        
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _allocationRepositoryMock.GetByIdAsync(session.AllocationId, Arg.Any<CancellationToken>())
            .Returns((Allocation?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotAuthorized()
    {
        // Arrange
        var command = new UpdateAttendanceSessionCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var teacherId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _allocationRepositoryMock.GetByIdAsync(session.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userContextMock.UserId.Returns(otherUserId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdatingSessionAndRecords()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        var studentId = Guid.NewGuid();
        
        var command = new UpdateAttendanceSessionCommand(session.Id, DateOnly.FromDateTime(DateTime.Now.AddDays(1)), new List<AttendanceRecordDto>
        {
            new AttendanceRecordDto(studentId, AttendanceStatus.Present)
        });
        
        _attendanceRepositoryMock.GetSessionByIdAsync(command.SessionId, Arg.Any<CancellationToken>())
            .Returns(session);
        _allocationRepositoryMock.GetByIdAsync(session.AllocationId, Arg.Any<CancellationToken>())
            .Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        
        // No duplicate for new date
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(allocation.Id, command.Date, Arg.Any<CancellationToken>())
            .Returns((AttendanceSession?)null);
            
        // Existing record null -> Create new
        _attendanceRepositoryMock.GetRecordBySessionAndStudentAsync(session.Id, studentId, Arg.Any<CancellationToken>())
            .Returns((AttendanceRecord?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _attendanceRepositoryMock.Received(1).UpdateSessionAsync(Arg.Is<AttendanceSession>(s => s.SessionDate == command.Date), Arg.Any<CancellationToken>());
        await _attendanceRepositoryMock.Received(1).AddRecordAsync(Arg.Is<AttendanceRecord>(r => r.StudentId == studentId && r.Status == AttendanceStatus.Present), Arg.Any<CancellationToken>());
    }
}