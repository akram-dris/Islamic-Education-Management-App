using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Domain.Entities;

public class AssignmentTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Update_ShouldReturnSuccess_WhenDataIsValid()
    {
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var result = assignment.Update("New", "New", DateOnly.MaxValue);
        result.IsSuccess.Should().BeTrue();
        assignment.Title.Should().Be("New");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenAllocationIdIsEmpty()
    {
        var result = Assignment.Create(Guid.Empty, "T", "D", DateOnly.MaxValue);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.EmptyAllocationId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        var result = Assignment.Create(Guid.NewGuid(), "", "D", DateOnly.MaxValue);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.EmptyTitle);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTitleIsWhitespace()
    {
        var result = Assignment.Create(Guid.NewGuid(), "   ", "D", DateOnly.MaxValue);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.EmptyTitle);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenTitleIsWhitespace()
    {
        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var result = assignment.Update("   ", "New", DateOnly.MaxValue);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Assignment.EmptyTitle);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Assignment);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = (Assignment)ctor!.Invoke(null);
        instance.Should().NotBeNull();

        var allocation = Allocation.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()).Value;
        instance.Allocation = allocation;
        instance.Allocation.Should().Be(allocation);
    }
}
