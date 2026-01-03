using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Domain.Entities;

public class AllocationTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTeacherIdIsEmpty()
    {
        var result = Allocation.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyTeacherId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenClassIdIsEmpty()
    {
        var result = Allocation.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyClassId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenSubjectIdIsEmpty()
    {
        var result = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptySubjectId);
    }

    [Fact]
    public void Update_ShouldReturnSuccess_WhenDataIsValid()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var newClassId = Guid.NewGuid();

        var result = allocation.Update(allocation.TeacherId, newClassId, allocation.SubjectId);

        result.IsSuccess.Should().BeTrue();
        allocation.ClassId.Should().Be(newClassId);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenTeacherIdIsEmpty()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var result = allocation.Update(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyTeacherId);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenClassIdIsEmpty()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var result = allocation.Update(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptyClassId);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenSubjectIdIsEmpty()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var result = allocation.Update(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Allocation.EmptySubjectId);
    }

    [Fact]
    public void Entity_ShouldHandleAuditProperties()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var now = DateTime.UtcNow;
        var userId = Guid.NewGuid();

        allocation.CreatedAt = now;
        allocation.CreatedBy = userId;
        allocation.LastModifiedAt = now;
        allocation.LastModifiedBy = userId;
        allocation.IsDeleted = true;

        allocation.CreatedAt.Should().Be(now);
        allocation.CreatedBy.Should().Be(userId);
        allocation.LastModifiedAt.Should().Be(now);
        allocation.LastModifiedBy.Should().Be(userId);
        allocation.IsDeleted.Should().BeTrue();

        var teacher = User.Create("u", "p", "f", QuranSchool.Domain.Enums.UserRole.Teacher).Value;
        var @class = Class.Create("C").Value;
        var subject = Subject.Create("S").Value;

        allocation.Teacher = teacher;
        allocation.Class = @class;
        allocation.Subject = subject;

        allocation.Teacher.Should().Be(teacher);
        allocation.Class.Should().Be(@class);
        allocation.Subject.Should().Be(subject);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Allocation);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        instance.Should().NotBeNull();
    }
}