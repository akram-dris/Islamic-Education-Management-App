using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SubmissionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Submission?> GetByStudentAndAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.AssignmentId == assignmentId, cancellationToken);
    }

    public async Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions
            .Where(s => s.AssignmentId == assignmentId)
            .Include(s => s.Student)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        await _dbContext.Submissions.AddAsync(submission, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _dbContext.Submissions.Update(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _dbContext.Submissions.Remove(submission);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Submissions.AnyAsync(s => s.StudentId == studentId && s.AssignmentId == assignmentId, cancellationToken);
    }
}
