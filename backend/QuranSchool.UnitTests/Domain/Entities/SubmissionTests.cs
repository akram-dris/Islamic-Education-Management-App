using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Errors;

namespace QuranSchool.UnitTests.Domain.Entities;

public class SubmissionTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GradeSubmission_ShouldReturnSuccess_WhenGradeIsValid()
    {
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var result = submission.GradeSubmission(90, "Good");
        result.IsSuccess.Should().BeTrue();
        submission.Grade.Should().Be(90);
    }

    [Fact]
    public void GradeSubmission_ShouldReturnFailure_WhenGradeIsInvalid()
    {
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var result = submission.GradeSubmission(110, "Invalid");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.InvalidGrade);
    }
}
