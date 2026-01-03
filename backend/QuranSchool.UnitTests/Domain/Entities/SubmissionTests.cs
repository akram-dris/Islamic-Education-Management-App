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
    public void Create_ShouldReturnFailure_WhenAssignmentIdIsEmpty()
    {
        var result = Submission.Create(Guid.Empty, Guid.NewGuid(), "url");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.EmptyAssignmentId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenStudentIdIsEmpty()
    {
        var result = Submission.Create(Guid.NewGuid(), Guid.Empty, "url");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.EmptyStudentId);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenFileUrlIsEmpty()
    {
        var result = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.EmptyFileUrl);
    }

    [Fact]
    public void GradeSubmission_ShouldReturnFailure_WhenGradeIsBelowZero()
    {
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var result = submission.GradeSubmission(-1, "Invalid");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.Submission.InvalidGrade);
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

    [Fact]
    public void Submission_ShouldHandleProperties()
    {
        var submission = Submission.Create(Guid.NewGuid(), Guid.NewGuid(), "url").Value;
        var now = DateTime.UtcNow;
        submission.CreatedAt = now;
        submission.CreatedAt.Should().Be(now);

        var assignment = Assignment.Create(Guid.NewGuid(), "T", "D", DateOnly.MaxValue).Value;
        var student = User.Create("u", "p", "f", QuranSchool.Domain.Enums.UserRole.Student).Value;

        submission.Assignment = assignment;
        submission.Student = student;

        submission.Assignment.Should().Be(assignment);
        submission.Student.Should().Be(student);
    }

    [Fact]
    public void PrivateConstructor_ShouldExist()
    {
        var type = typeof(Submission);
        var ctor = type.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        var instance = ctor!.Invoke(null);
        instance.Should().NotBeNull();
    }
}