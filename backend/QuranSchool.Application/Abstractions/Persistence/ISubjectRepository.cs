using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Subject>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Subject subject, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default);
    Task DeleteAsync(Subject subject, CancellationToken cancellationToken = default);
}
