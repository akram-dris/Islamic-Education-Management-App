using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IAllocationRepository
{
    Task<Allocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Allocation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Allocation>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
    Task AddAsync(Allocation allocation, CancellationToken cancellationToken = default);
    Task DeleteAsync(Allocation allocation, CancellationToken cancellationToken = default);
}
