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

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenSessionCreationFails()
    {
        var teacherId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var command = new MarkAttendanceCommand(allocation.Id, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        
        // AllocationId is Empty -> Session Creation Fails
        // Wait, allocation.Id is already set. Need a command with Empty AllocationId but that would fail GetById.
        // Actually, AttendanceSession.Create only fails if allocationId is Empty.
        // Let's mock GetById to return an allocation with Empty Id if possible (unlikely via Create).
        // Better: mock repository to return a valid allocation, but pass Guid.Empty in command if logic allows.
        // Looking at Handler: var allocation = await _allocationRepository.GetByIdAsync(request.AllocationId, cancellationToken);
        // If request.AllocationId is Guid.Empty, repository likely returns null.
        // To hit sessionResult.IsFailure, we need request.AllocationId to be Guid.Empty BUT repository to return non-null.
        
        var invalidAllocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        typeof(Allocation).GetProperty(nameof(Allocation.Id))?.SetValue(invalidAllocation, Guid.Empty);

        var command2 = new MarkAttendanceCommand(Guid.Empty, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        _allocationRepositoryMock.GetByIdAsync(Guid.Empty).Returns(invalidAllocation);
        _userContextMock.UserId.Returns(invalidAllocation.TeacherId);

        var result = await _handler.Handle(command2, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceSession.EmptyAllocationId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenRecordCreationFails()
    {
        var teacherId = Guid.NewGuid();
        var allocation = Allocation.Create(teacherId, Guid.NewGuid(), Guid.NewGuid()).Value;
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        var command = new MarkAttendanceCommand(allocation.Id, session.SessionDate, new List<AttendanceRecordDto>
        {
            new AttendanceRecordDto(Guid.Empty, AttendanceStatus.Present)
        });

        _allocationRepositoryMock.GetByIdAsync(command.AllocationId).Returns(allocation);
        _userContextMock.UserId.Returns(teacherId);
        _attendanceRepositoryMock.GetSessionByAllocationAndDateAsync(allocation.Id, command.Date).Returns(session);

        var result = await _handler.Handle(command, default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceRecord.EmptyStudentId);
    }
}