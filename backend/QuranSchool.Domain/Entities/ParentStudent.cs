using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class ParentStudent : Entity
{
    private ParentStudent(Guid parentId, Guid studentId)
    {
        ParentId = parentId;
        StudentId = studentId;
    }

    private ParentStudent()
    {
    }

    public Guid ParentId { get; private set; }
    public Guid StudentId { get; private set; }

    public User? Parent { get; set; }
    public User? Student { get; set; }

    public static Result<ParentStudent> Create(Guid parentId, Guid studentId)
    {
        if (parentId == Guid.Empty)
        {
            return Result<ParentStudent>.Failure(DomainErrors.User.EmptyParentId);
        }

        if (studentId == Guid.Empty)
        {
            return Result<ParentStudent>.Failure(DomainErrors.User.EmptyStudentIdForLink);
        }

        return new ParentStudent(parentId, studentId);
    }
}