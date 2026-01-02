using FluentAssertions;
using NSubstitute;
using Xunit;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Application.Features.Attendance.GetMy;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Abstractions;

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
    public async Task Handle_ShouldReturnAttendanceForUser()
    {
        var userId = Guid.NewGuid();
        var query = new GetMyAttendanceQuery(userId);
        
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        allocation.Class = Class.Create("C").Value;
        allocation.Subject = Subject.Create("S").Value;
        
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        session.Allocation = allocation;
        
        var record = AttendanceRecord.Create(session.Id, userId, AttendanceStatus.Present).Value;
        record.AttendanceSession = session;
        
        var records = new List<AttendanceRecord> { record };
        
        _attendanceRepositoryMock.GetRecordsByStudentIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(records);

        var result = await _handler.Handle(query, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
    }
}