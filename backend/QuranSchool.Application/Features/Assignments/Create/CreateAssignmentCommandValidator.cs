using FluentValidation;
using QuranSchool.Application.Features.Assignments.Create;

namespace QuranSchool.Application.Features.Assignments.Create;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.AllocationId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DueDate).NotEmpty().GreaterThan(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
    }
}
