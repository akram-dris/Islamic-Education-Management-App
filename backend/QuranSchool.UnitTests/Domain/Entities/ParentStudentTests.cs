using FluentAssertions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Entities;

public class ParentStudentTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenParentIdIsEmpty()
    {
        var result = ParentStudent.Create(Guid.Empty, Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyParentId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenStudentIdIsEmpty()
    {
        var result = ParentStudent.Create(Guid.NewGuid(), Guid.Empty);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyStudentIdForLink);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var parentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var result = ParentStudent.Create(parentId, studentId);
        result.IsSuccess.Should().BeTrue();
        result.Value.ParentId.Should().Be(parentId);
        result.Value.StudentId.Should().Be(studentId);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(ParentStudent);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = (ParentStudent)ctor!.Invoke(null);
        instance.Should().NotBeNull();

        var parent = User.Create("p", "p", "f", QuranSchool.Domain.Enums.UserRole.Parent).Value;
        var student = User.Create("s", "p", "f", QuranSchool.Domain.Enums.UserRole.Student).Value;
        instance.Parent = parent;
        instance.Student = student;
        instance.Parent.Should().Be(parent);
        instance.Student.Should().Be(student);
    }
}
