using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AssignmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .Include(a => a.Allocation)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Assignment>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .Where(a => _dbContext.Allocations.Any(al => al.Id == a.AllocationId && al.ClassId == classId))
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Assignment>> GetByAllocationIdAsync(Guid allocationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Assignments
            .Where(a => a.AllocationId == allocationId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Assignments.AddAsync(assignment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
