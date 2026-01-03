using FluentValidation.TestHelper;
using QuranSchool.Application.Features.Assignments.Create;
using Xunit;

namespace QuranSchool.UnitTests.Features.Assignments.Create;

public class CreateAssignmentCommandValidatorTests
{
    private readonly CreateAssignmentCommandValidator _validator;

    public CreateAssignmentCommandValidatorTests()
    {
        _validator = new CreateAssignmentCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_AllocationIdIsEmpty()
    {
        var command = new CreateAssignmentCommand(Guid.Empty, "Title", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AllocationId);
    }

    [Fact]
    public void Should_HaveError_When_TitleIsEmpty()
    {
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_HaveError_When_DueDateIsInPast()
    {
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "Title", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(-2)));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateAssignmentCommand(Guid.NewGuid(), "Title", "Desc", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
