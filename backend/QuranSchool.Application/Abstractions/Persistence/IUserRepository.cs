using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task AddParentStudentLinkAsync(ParentStudent parentStudent, CancellationToken cancellationToken = default);
    Task<bool> IsParentLinkedAsync(Guid parentId, Guid studentId, CancellationToken cancellationToken = default);
    Task<List<User>> GetAllByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
}
