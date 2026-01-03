using FluentAssertions;
using QuranSchool.Domain.Abstractions;
using Xunit;

namespace QuranSchool.UnitTests.Domain.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_ShouldSetIsSuccessToTrue()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldSetIsSuccessToFalse()
    {
        var error = Error.Validation("Code", "Description");
        var result = Result.Failure(error);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenSuccessAndErrorNotNull()
    {
        var action = () => {
            var type = typeof(Result);
            var ctor = type.GetConstructor(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, new[] { typeof(bool), typeof(Error) }, null);
            ctor!.Invoke(new object[] { true, Error.Validation("C", "D") });
        };

        action.Should().Throw<System.Reflection.TargetInvocationException>()
            .WithInnerException<ArgumentException>();
    }

    [Fact]
    public void Value_ShouldThrowException_WhenFailure()
    {
        var result = Result<int>.Failure(Error.Validation("C", "D"));
        var action = () => result.Value;
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitOperator_ShouldReturnFailure_WhenValueIsNull()
    {
        string? value = null;
        Result<string> result = value;
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }

    [Fact]
    public void ImplicitOperator_ShouldReturnSuccess_WhenValueIsNotNull()
    {
        string value = "test";
        Result<string> result = value;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ValidationResult_WithErrors_ShouldSetErrors()
    {
        var errors = new[] { Error.Validation("C1", "D1"), Error.Validation("C2", "D2") };
        var result = ValidationResult.WithErrors(errors);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void GenericValidationResult_WithErrors_ShouldSetErrors()
    {
        var errors = new[] { Error.Validation("C1", "D1") };
        var result = ValidationResult<int>.WithErrors(errors);
        result.Errors.Should().BeEquivalentTo(errors);
    }
}
