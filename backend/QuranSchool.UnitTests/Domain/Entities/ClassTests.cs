using FluentAssertions;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Entities;

public class ClassTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var result = Class.Create("");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.EmptyName);
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var @class = Class.Create("Name").Value;
        var result = @class.Update("");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Class.EmptyName);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Class);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        instance.Should().NotBeNull();
    }
}
