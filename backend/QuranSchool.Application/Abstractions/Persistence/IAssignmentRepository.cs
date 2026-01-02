using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Assignment>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
    Task<List<Assignment>> GetByAllocationIdAsync(Guid allocationId, CancellationToken cancellationToken = default);
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
}
