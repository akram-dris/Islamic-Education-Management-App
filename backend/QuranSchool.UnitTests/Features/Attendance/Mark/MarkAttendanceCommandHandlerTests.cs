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
        _handler = new MarkAttendanceCommandHandler(_attendanceRepositoryMock, _allocationRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenAllocationNotFound()
    {
        // Arrange
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns((Allocation?)null);

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
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(Guid.NewGuid()); // Different user ID

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.NotAuthorized);
    }

    [Fact]
    public async Task Handle_ShouldCreateSessionAndRecord_WhenSessionAndRecordDoNotExist()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto> 
        { 
            new(studentId, AttendanceStatus.Present) 
        });
        
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        
        // Session does not exist
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(command.AllocationId, command.Date, Arg.Any<CancellationToken>())
            .Returns((AttendanceSession?)null);
            
        // Record does not exist (implied because session is new, but logic checks anyway if session id was retrieved)
        // Since session is null, code creates new session. 
        // Then it loops records. 
        
        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _attendanceRepositoryMock.Received(1).AddSessionAsync(Arg.Any<AttendanceSession>(), Arg.Any<CancellationToken>());
        await _attendanceRepositoryMock.Received(1).AddRecordAsync(Arg.Any<AttendanceRecord>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateRecord_WhenRecordExists()
    {
        // Arrange
        var teacherId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto> 
        { 
            new(studentId, AttendanceStatus.Absent) 
        });
        
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(command.AllocationId, command.Date).Value;
        var existingRecord = AttendanceRecord.Create(session.Id, studentId, AttendanceStatus.Present).Value;

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(command.AllocationId, command.Date, Arg.Any<CancellationToken>())
            .Returns(session);
            
        _attendanceRepositoryMock.GetRecordBySessionAndStudentAsync(session.Id, studentId, Arg.Any<CancellationToken>())
            .Returns(existingRecord);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _attendanceRepositoryMock.Received(1).UpdateRecordAsync(Arg.Is<AttendanceRecord>(r => r.Status == AttendanceStatus.Absent), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSessionCreationFails()
    {
        // Arrange
        // To fail session creation, AllocationId must be empty. 
        // But if AllocationId is empty, GetById might fail or return null?
        // We will mock GetById to return a valid allocation even if Guid is empty, for testing purposes.
        var teacherId = Guid.NewGuid();
        var command = new MarkAttendanceCommand(Guid.Empty, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        
        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(command.AllocationId, command.Date, Arg.Any<CancellationToken>())
            .Returns((AttendanceSession?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceSession.EmptyAllocationId);
    }
}
