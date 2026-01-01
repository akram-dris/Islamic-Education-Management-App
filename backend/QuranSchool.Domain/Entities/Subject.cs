using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Subject : Entity
{
    public string Name { get; set; } = string.Empty;
}
