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
}
