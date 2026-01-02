using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Submission?> GetByStudentAndAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task AddAsync(Submission submission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
}
