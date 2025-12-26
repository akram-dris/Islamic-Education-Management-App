using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Attendance.Mark;

public class MarkAttendanceCommandHandlerTests
{
    private readonly IAttendanceRepository _attendanceRepositoryMock;
    private readonly IAllocationRepository _allocationRepositoryMock;
    private readonly MarkAttendanceCommandHandler _handler;

    public MarkAttendanceCommandHandlerTests()
    {
        _attendanceRepositoryMock = Substitute.For<IAttendanceRepository>();
        _allocationRepositoryMock = Substitute.For<IAllocationRepository>();
        _handler = new MarkAttendanceCommandHandler(_attendanceRepositoryMock, _allocationRepositoryMock);
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
        result.Error.Code.Should().Be("Allocation.NotFound");
    }
}
