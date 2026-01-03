using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using QuranSchool.Application.Abstractions.Behaviors;
using QuranSchool.Domain.Abstractions;
using Xunit;

namespace QuranSchool.UnitTests.Abstractions.Behaviors;

public class ValidationBehaviorTests
{
    private readonly IEnumerable<IValidator<TestRequest>> _validators;
    private readonly IValidator<TestRequest> _validatorMock;
    private readonly ValidationBehavior<TestRequest, Result> _behavior;

    public ValidationBehaviorTests()
    {
        _validatorMock = Substitute.For<IValidator<TestRequest>>();
        _validators = new[] { _validatorMock };
        _behavior = new ValidationBehavior<TestRequest, Result>(_validators);
    }

    public class TestRequest : IRequest<Result> { }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidators()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestRequest, Result>(Enumerable.Empty<IValidator<TestRequest>>());
        var next = Substitute.For<RequestHandlerDelegate<Result>>();
        next().Returns(Result.Success());

        // Act
        var result = await behavior.Handle(new TestRequest(), next, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await next.Received(1)();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationResult_WhenValidationFails()
    {
        // Arrange
        var next = Substitute.For<RequestHandlerDelegate<Result>>();
        var failures = new List<ValidationFailure> { new("Prop", "Error") };
        _validatorMock.Validate(Arg.Any<ValidationContext<TestRequest>>())
            .Returns(new FluentValidation.Results.ValidationResult(failures));

        // Act
        var result = await _behavior.Handle(new TestRequest(), next, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Should().BeOfType<QuranSchool.Domain.Abstractions.ValidationResult>();
        ((IValidationResult)result).Errors.Should().ContainSingle();
        await next.DidNotReceive()();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        // Arrange
        var next = Substitute.For<RequestHandlerDelegate<Result>>();
        next().Returns(Result.Success());
        _validatorMock.Validate(Arg.Any<ValidationContext<TestRequest>>())
            .Returns(new FluentValidation.Results.ValidationResult());

        // Act
        var result = await _behavior.Handle(new TestRequest(), next, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await next.Received(1)();
    }

    public class TestRequestGeneric : IRequest<Result<int>> { }

    [Fact]
    public async Task Handle_ShouldReturnGenericValidationResult_WhenValidationFails()
    {
        // Arrange
        var validatorMock = Substitute.For<IValidator<TestRequestGeneric>>();
        var behavior = new ValidationBehavior<TestRequestGeneric, Result<int>>(new[] { validatorMock });
        var next = Substitute.For<RequestHandlerDelegate<Result<int>>>();
        
        var failures = new List<ValidationFailure> { new("Prop", "Error") };
        validatorMock.Validate(Arg.Any<ValidationContext<TestRequestGeneric>>())
            .Returns(new FluentValidation.Results.ValidationResult(failures));

        // Act
        var result = await behavior.Handle(new TestRequestGeneric(), next, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Should().BeOfType<ValidationResult<int>>();
        ((IValidationResult)result).Errors.Should().ContainSingle();
    }
}
