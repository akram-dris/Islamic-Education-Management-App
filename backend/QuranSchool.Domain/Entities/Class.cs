using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Class : Entity
{
    public string Name { get; set; } = string.Empty;
}
