using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Domain.Entities;

public class AttendanceTests
{
    [Fact]
    public void CreateSession_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = AttendanceSession.Create(Guid.NewGuid(), DateOnly.MaxValue);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CreateRecord_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = AttendanceRecord.Create(Guid.NewGuid(), Guid.NewGuid(), AttendanceStatus.Present);
        result.IsSuccess.Should().BeTrue();
    }
}
