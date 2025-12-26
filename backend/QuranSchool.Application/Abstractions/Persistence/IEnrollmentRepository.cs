using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid classId, CancellationToken cancellationToken = default);
}
