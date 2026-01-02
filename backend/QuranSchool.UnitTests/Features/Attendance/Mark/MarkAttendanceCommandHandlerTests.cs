using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Features.Attendance.Mark;

public class MarkAttendanceCommandHandlerTests
{
    private readonly IAttendanceRepository _attendanceRepositoryMock;
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly IUserContext _userContextMock;
    private readonly MarkAttendanceCommandHandler _handler;

    public MarkAttendanceCommandHandlerTests()
    {
        _attendanceRepositoryMock = Substitute.For<IAttendanceRepository>();
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new MarkAttendanceCommandHandler(
            _attendanceRepositoryMock,
            _allocationRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns((Allocation?)null);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserNotAuthorized()
    {
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(Guid.NewGuid()); // Different user

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }

    [Fact]
    public async Task Handle_ShouldCreateSessionAndRecords_WhenSessionDoesNotExist()
    {
        var teacherId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var studentId = Guid.NewGuid();
        var command = new MarkAttendanceCommand(allocation.Id, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>
        {
            new AttendanceRecordDto(studentId, AttendanceStatus.Present)
        });

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(allocation.Id, command.Date).Returns((AttendanceSession?)null);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        await _attendanceRepositoryMock.Received(1).AddSessionAsync(Arg.Any<AttendanceSession>(), Arg.Any<CancellationToken>());
        await _attendanceRepositoryMock.Received(1).AddRecordAsync(Arg.Any<AttendanceRecord>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenRecordAlreadyExists()
    {
        var teacherId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        var studentId = Guid.NewGuid();
        var existingRecord = AttendanceRecord.Create(session.Id, studentId, AttendanceStatus.Absent).Value;
        
        var command = new MarkAttendanceCommand(allocation.Id, session.SessionDate, new List<AttendanceRecordDto>
        {
            new AttendanceRecordDto(studentId, AttendanceStatus.Present)
        });

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(allocation.Id, command.Date).Returns(session);
        _attendanceRepositoryMock.GetRecordBySessionAndStudentAsync(session.Id, studentId).Returns(existingRecord);

        var result = await _handler.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
        existingRecord.Status.Should().Be(AttendanceStatus.Present);
        await _attendanceRepositoryMock.Received(1).UpdateRecordAsync(existingRecord, Arg.Any<CancellationToken>());
    }
}