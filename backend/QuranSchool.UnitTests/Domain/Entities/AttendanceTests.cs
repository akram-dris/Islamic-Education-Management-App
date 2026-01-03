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

    [Fact]
    public void CreateSession_ShouldReturnFailure_WhenAllocationIdIsEmpty()
    {
        var result = AttendanceSession.Create(Guid.Empty, DateOnly.MaxValue);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceSession.EmptyAllocationId);
    }

    [Fact]
    public void CreateRecord_ShouldReturnFailure_WhenSessionIdIsEmpty()
    {
        var result = AttendanceRecord.Create(Guid.Empty, Guid.NewGuid(), AttendanceStatus.Present);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceRecord.EmptySessionId);
    }

    [Fact]
    public void CreateRecord_ShouldReturnFailure_WhenStudentIdIsEmpty()
    {
        var result = AttendanceRecord.Create(Guid.NewGuid(), Guid.Empty, AttendanceStatus.Present);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.AttendanceRecord.EmptyStudentId);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var typeSession = typeof(AttendanceSession);
        var ctorSession = typeSession.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instanceSession = (AttendanceSession)ctorSession!.Invoke(null);
        instanceSession.Should().NotBeNull();

        var typeRecord = typeof(AttendanceRecord);
        var ctorRecord = typeRecord.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instanceRecord = (AttendanceRecord)ctorRecord!.Invoke(null);
        instanceRecord.Should().NotBeNull();

        // Exercise navigations
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        instanceSession.Allocation = allocation;
        instanceSession.Allocation.Should().Be(allocation);

        var student = User.Create("u", "p", "f", QuranSchool.Domain.Enums.UserRole.Student).Value;
        instanceRecord.AttendanceSession = instanceSession;
        instanceRecord.Student = student;
        instanceRecord.AttendanceSession.Should().Be(instanceSession);
        instanceRecord.Student.Should().Be(student);
    }
}
