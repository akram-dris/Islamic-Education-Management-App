using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Submission : Entity
{
    private Submission(Guid assignmentId, Guid studentId, string fileUrl)
    {
        AssignmentId = assignmentId;
        StudentId = studentId;
        FileUrl = fileUrl;
    }

    private Submission()
    {
    }

    public Guid AssignmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public string FileUrl { get; private set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; }

    public Assignment? Assignment { get; set; }
    public User? Student { get; set; }

    public static Result<Submission> Create(Guid assignmentId, Guid studentId, string fileUrl)
    {
        if (assignmentId == Guid.Empty)
        {
            return Result<Submission>.Failure(DomainErrors.Submission.EmptyAssignmentId);
        }

        if (studentId == Guid.Empty)
        {
            return Result<Submission>.Failure(DomainErrors.Submission.EmptyStudentId);
        }

        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Result<Submission>.Failure(DomainErrors.Submission.EmptyFileUrl);
        }

        return new Submission(assignmentId, studentId, fileUrl);
    }

    public Result GradeSubmission(decimal grade, string? remarks)
    {
        if (grade < 0 || grade > 100)
        {
            return Result.Failure(DomainErrors.Submission.InvalidGrade);
        }

        Grade = grade;
        Remarks = remarks;

        return Result.Success();
    }
}
