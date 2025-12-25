namespace QuranSchool.Domain.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public Guid AllocationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Allocation? Allocation { get; set; }
}
