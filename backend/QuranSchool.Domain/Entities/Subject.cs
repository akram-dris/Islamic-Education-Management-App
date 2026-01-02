using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Subject : Entity
{
    private Subject(string name)
    {
        Name = name;
    }

    private Subject()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public static Result<Subject> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Subject>.Failure(DomainErrors.Subject.EmptyName);
        }

        return new Subject(name);
    }

    public Result Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(DomainErrors.Subject.EmptyName);
        }

        Name = name;

        return Result.Success();
    }
}