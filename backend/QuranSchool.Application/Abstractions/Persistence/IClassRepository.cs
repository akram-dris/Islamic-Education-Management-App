using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Class>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Class @class, CancellationToken cancellationToken = default);
    Task UpdateAsync(Class @class, CancellationToken cancellationToken = default);
    Task DeleteAsync(Class @class, CancellationToken cancellationToken = default);
}
