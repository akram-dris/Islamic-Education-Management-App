using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddParentStudentLinkAsync(ParentStudent parentStudent, CancellationToken cancellationToken = default)
    {
        await _dbContext.ParentStudents.AddAsync(parentStudent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsParentLinkedAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ParentStudents.AnyAsync(ps => ps.ParentId == parentId && ps.StudentId == studentId, cancellationToken);
    }

    public async Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);
    }
}
