using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Enrollment : Entity
{
    private Enrollment(Guid studentId, Guid classId)
    {
        StudentId = studentId;
        ClassId = classId;
    }

    private Enrollment()
    {
    }

    public Guid StudentId { get; private set; }
    public Guid ClassId { get; private set; }

    public User? Student { get; set; }
    public Class? Class { get; set; }

    public static Result<Enrollment> Create(Guid studentId, Guid classId)
    {
        if (studentId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(DomainErrors.Enrollment.EmptyStudentId);
        }

        if (classId == Guid.Empty)
        {
            return Result<Enrollment>.Failure(DomainErrors.Enrollment.EmptyClassId);
        }

        return new Enrollment(studentId, classId);
    }
}
