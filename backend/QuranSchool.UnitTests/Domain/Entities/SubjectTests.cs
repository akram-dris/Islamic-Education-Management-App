using FluentAssertions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Entities;

public class SubjectTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var result = Subject.Create("");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.EmptyName);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var subject = Subject.Create("Name").Value;
        var result = subject.Update("");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Subject.EmptyName);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Subject);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        instance.Should().NotBeNull();
    }
}
