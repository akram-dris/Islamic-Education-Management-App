using FluentValidation.TestHelper;
using QuranSchool.Application.Features.Users.Register;
using QuranSchool.Domain.Enums;
using Xunit;

namespace QuranSchool.UnitTests.Features.Users.Register;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_UsernameIsEmpty()
    {
        var command = new RegisterUserCommand("", "Password123", "Full Name", UserRole.Student);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_HaveError_When_UsernameIsTooLong()
    {
        var command = new RegisterUserCommand(new string('a', 51), "Password123", "Full Name", UserRole.Student);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_HaveError_When_PasswordIsTooShort()
    {
        var command = new RegisterUserCommand("username", "12345", "Full Name", UserRole.Student);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_HaveError_When_FullNameIsEmpty()
    {
        var command = new RegisterUserCommand("username", "Password123", "", UserRole.Student);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new RegisterUserCommand("username", "Password123", "Full Name", UserRole.Student);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
