using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Class : Entity
{
    private Class(string name)
    {
        Name = name;
    }

    private Class()
    {
    }

    public string Name { get; private set; } = string.Empty;

    public static Result<Class> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Class>.Failure(DomainErrors.Class.EmptyName);
        }

        return new Class(name);
    }
}
