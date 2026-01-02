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
    public void Update_ShouldReturnSuccess_WhenDataIsValid()
    {
        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        var newClassId = Guid.NewGuid();

        var result = allocation.Update(allocation.TeacherId, newClassId, allocation.SubjectId);

        result.IsSuccess.Should().BeTrue();
        allocation.ClassId.Should().Be(newClassId);
    }
}
