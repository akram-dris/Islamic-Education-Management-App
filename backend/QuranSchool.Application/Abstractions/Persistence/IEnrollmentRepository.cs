using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid classId, CancellationToken cancellationToken = default);
}
