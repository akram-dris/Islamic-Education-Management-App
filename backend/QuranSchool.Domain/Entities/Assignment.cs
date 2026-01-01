using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Entities;

public class Assignment : Entity
{
    public Guid AllocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DueDate { get; set; }

    public Allocation? Allocation { get; set; }
}
