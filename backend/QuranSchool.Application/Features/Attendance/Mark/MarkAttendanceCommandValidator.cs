using FluentValidation;
using QuranSchool.Application.Features.Attendance.Mark;

namespace QuranSchool.Application.Features.Attendance.Mark;

public class MarkAttendanceCommandValidator : AbstractValidator<MarkAttendanceCommand>
{
    public MarkAttendanceCommandValidator()
    {
        RuleFor(x => x.AllocationId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Records).NotEmpty();
        RuleForEach(x => x.Records).ChildRules(record => 
        {
            record.RuleFor(r => r.StudentId).NotEmpty();
            record.RuleFor(r => r.Status).IsInEnum();
        });
    }
}
