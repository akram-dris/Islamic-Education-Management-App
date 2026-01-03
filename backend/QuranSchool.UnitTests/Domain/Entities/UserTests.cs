using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = User.Create("u", "p", "f", UserRole.Student);

        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("u");
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenUsernameIsEmpty()
    {
        var result = User.Create("", "p", "f", UserRole.Student);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyUsername);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenPasswordIsEmpty()
    {
        var result = User.Create("u", "", "f", UserRole.Student);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyPassword);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFullNameIsEmpty()
    {
        var result = User.Create("u", "p", "", UserRole.Student);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyFullName);
    }

    [Fact]
    public void Update_ShouldReturnSuccess_WhenDataIsValid()
    {
        var user = User.Create("u", "p", "f", UserRole.Student).Value;

        var result = user.Update("new", "newpass");

        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("new");
        user.PasswordHash.Should().Be("newpass");
    }

    [Fact]
    public void Update_ShouldNotUpdatePassword_WhenPasswordIsEmpty()
    {
        var user = User.Create("u", "p", "f", UserRole.Student).Value;

        var result = user.Update("new", "");

        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("new");
        user.PasswordHash.Should().Be("p"); // Remains old password
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(User);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        instance.Should().NotBeNull();
    }
}
