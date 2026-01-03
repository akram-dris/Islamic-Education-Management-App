using FluentValidation.TestHelper;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Domain.Enums;
using Xunit;

namespace QuranSchool.UnitTests.Features.Attendance.Mark;

public class MarkAttendanceCommandValidatorTests
{
    private readonly MarkAttendanceCommandValidator _validator;

    public MarkAttendanceCommandValidatorTests()
    {
        _validator = new MarkAttendanceCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_AllocationIdIsEmpty()
    {
        var command = new MarkAttendanceCommand(Guid.Empty, DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AllocationId);
    }

    [Fact]
    public void Should_HaveError_When_RecordsAreEmpty()
    {
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), new List<AttendanceRecordDto>());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Records);
    }

    [Fact]
    public void Should_HaveError_When_StudentRecordIsInvalid()
    {
        var records = new List<AttendanceRecordDto>
        {
            new(Guid.Empty, AttendanceStatus.Present)
        };
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), records);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("Records[0].StudentId");
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var records = new List<AttendanceRecordDto>
        {
            new(Guid.NewGuid(), AttendanceStatus.Present)
        };
        var command = new MarkAttendanceCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), records);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
