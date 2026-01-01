using QuranSchool.Domain.Abstractions;
using QuranSchool.Domain.Errors;

namespace QuranSchool.Domain.Entities;

public class Assignment : Entity
{
    private Assignment(Guid allocationId, string title, string? description, DateOnly dueDate)
    {
        AllocationId = allocationId;
        Title = title;
        Description = description;
        DueDate = dueDate;
    }

    private Assignment()
    {
    }

    public Guid AllocationId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly DueDate { get; private set; }
  

    public Allocation? Allocation { get; set; }

    public static Result<Assignment> Create(Guid allocationId, string title, string? description, DateOnly dueDate)
    {
        if (allocationId == Guid.Empty)
        {
            return Result<Assignment>.Failure(DomainErrors.Assignment.EmptyAllocationId);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Assignment>.Failure(DomainErrors.Assignment.EmptyTitle);
        }

        return new Assignment(allocationId, title, description, dueDate);
    }
}
