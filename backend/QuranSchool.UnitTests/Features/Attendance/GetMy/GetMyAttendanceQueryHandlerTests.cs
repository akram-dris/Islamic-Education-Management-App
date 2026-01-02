using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Authentication;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.GetMy;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.UnitTests.Features.Attendance.GetMy;

public class GetMyAttendanceQueryHandlerTests
{
    private readonly IAttendanceRepository _attendanceRepositoryMock;
    private readonly GetMyAttendanceQueryHandler _handler;

    public GetMyAttendanceQueryHandlerTests()
    {
        _attendanceRepositoryMock = Substitute.For<IAttendanceRepository>();
        _handler = new GetMyAttendanceQueryHandler(_attendanceRepositoryMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnAttendanceRecords_ForCurrentUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var session = AttendanceSession.Create(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now)).Value;
        
        // Mock navigation properties for Session -> Allocation -> Class & Subject
        var cls = Class.Create("Class A").Value;
        var subject = Subject.Create("Subject 1").Value;
        var allocation = Allocation.Create(Guid.NewGuid(), cls.Id, subject.Id).Value;
        
        // We need deep mocking or reflection setting for these nested properties because Update/Create methods don't set nav properties.
        typeof(Allocation).GetProperty(nameof(Allocation.Class))!.SetValue(allocation, cls);
        typeof(Allocation).GetProperty(nameof(Allocation.Subject))!.SetValue(allocation, subject);
        
        typeof(AttendanceSession).GetProperty(nameof(AttendanceSession.Allocation))!.SetValue(session, allocation);
        
        var record = AttendanceRecord.Create(session.Id, userId, AttendanceStatus.Present).Value;
        typeof(AttendanceRecord).GetProperty(nameof(AttendanceRecord.AttendanceSession))!.SetValue(record, session);

        _attendanceRepositoryMock.GetRecordsByStudentIdAsync(userId, default).Returns(new List<AttendanceRecord> { record });

        // Act
        var result = await _handler.Handle(new GetMyAttendanceQuery(userId), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Status.Should().Be(AttendanceStatus.Present);
        result.Value[0].Date.Should().Be(session.SessionDate);
        result.Value[0].ClassName.Should().Be("Class A");
    }
}
