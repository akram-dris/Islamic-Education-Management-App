using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Allocation : Entity
{
    private Allocation(Guid teacherId, Guid classId, Guid subjectId)
    {
        TeacherId = teacherId;
        ClassId = classId;
        SubjectId = subjectId;
    }

    private Allocation()
    {
    }

    public Guid TeacherId { get; private set; }
    public Guid ClassId { get; private set; }
    public Guid SubjectId { get; private set; }

    public User? Teacher { get; set; }
    public Class? Class { get; set; }
    public Subject? Subject { get; set; }

    public static Result<Allocation> Create(Guid teacherId, Guid classId, Guid subjectId)
    {
        if (teacherId == Guid.Empty)
        {
            return Result<Allocation>.Failure(DomainErrors.Allocation.EmptyTeacherId);
        }

        if (classId == Guid.Empty)
        {
            return Result<Allocation>.Failure(DomainErrors.Allocation.EmptyClassId);
        }

        if (subjectId == Guid.Empty)
        {
            return Result<Allocation>.Failure(DomainErrors.Allocation.EmptySubjectId);
        }

        return new Allocation(teacherId, classId, subjectId);
    }

    public Result Update(Guid teacherId, Guid classId, Guid subjectId)
    {
        if (teacherId == Guid.Empty)
        {
            return Result.Failure(DomainErrors.Allocation.EmptyTeacherId);
        }

        if (classId == Guid.Empty)
        {
            return Result.Failure(DomainErrors.Allocation.EmptyClassId);
        }

        if (subjectId == Guid.Empty)
        {
            return Result.Failure(DomainErrors.Allocation.EmptySubjectId);
        }

        TeacherId = teacherId;
        ClassId = classId;
        SubjectId = subjectId;

        return Result.Success();
    }
}