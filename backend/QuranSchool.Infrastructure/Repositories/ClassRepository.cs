using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ClassRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Class?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Classes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<List<Class>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Classes.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Class @class, CancellationToken cancellationToken = default)
    {
        await _dbContext.Classes.AddAsync(@class, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Class @class, CancellationToken cancellationToken = default)
    {
        _dbContext.Classes.Update(@class);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Class @class, CancellationToken cancellationToken = default)
    {
        _dbContext.Classes.Remove(@class);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
