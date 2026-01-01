using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class AllocationRepository : IAllocationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AllocationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Allocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Allocations
            .Include(a => a.Teacher)
            .Include(a => a.Class)
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Allocation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Allocations
            .Include(a => a.Teacher)
            .Include(a => a.Class)
            .Include(a => a.Subject)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Allocation>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Allocations
            .Include(a => a.Class)
            .Include(a => a.Subject)
            .Where(a => a.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Allocation allocation, CancellationToken cancellationToken = default)
    {
        await _dbContext.Allocations.AddAsync(allocation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Allocation allocation, CancellationToken cancellationToken = default)
    {
        _dbContext.Allocations.Update(allocation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid teacherId, Guid classId, Guid subjectId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Allocations.AnyAsync(a => 
            a.TeacherId == teacherId && 
            a.ClassId == classId && 
            a.SubjectId == subjectId, 
            cancellationToken);
    }

    public async Task DeleteAsync(Allocation allocation, CancellationToken cancellationToken = default)
    {
        _dbContext.Allocations.Remove(allocation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
