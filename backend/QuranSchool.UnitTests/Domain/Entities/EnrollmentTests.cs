using FluentAssertions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Entities;

public class EnrollmentTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenStudentIdIsEmpty()
    {
        var result = Enrollment.Create(Guid.Empty, Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Enrollment.EmptyStudentId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenClassIdIsEmpty()
    {
        var result = Enrollment.Create(Guid.NewGuid(), Guid.Empty);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Enrollment.EmptyClassId);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var result = Enrollment.Create(studentId, classId);
        result.IsSuccess.Should().BeTrue();
        result.Value.StudentId.Should().Be(studentId);
        result.Value.ClassId.Should().Be(classId);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Enrollment);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = (Enrollment)ctor!.Invoke(null);
        instance.Should().NotBeNull();

        var student = User.Create("u", "p", "f", QuranSchool.Domain.Enums.UserRole.Student).Value;
        var @class = Class.Create("C").Value;
        instance.Student = student;
        instance.Class = @class;
        instance.Student.Should().Be(student);
        instance.Class.Should().Be(@class);
    }
}
