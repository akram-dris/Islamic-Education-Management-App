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
    public void Update_ShouldReturnSuccess_WhenDataIsValid()
    {
        var user = User.Create("u", "p", "f", UserRole.Student).Value;

        var result = user.Update("new", "newpass");

        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be("new");
        user.PasswordHash.Should().Be("newpass");
    }

    [Fact]
    public void Update_ShouldReturnFailure_WhenFullNameIsEmpty()
    {
        var user = User.Create("u", "p", "f", UserRole.Student).Value;

        var result = user.Update("", null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyFullName);
    }
}
