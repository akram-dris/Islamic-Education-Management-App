using Microsoft.EntityFrameworkCore;
using QuranSchool.Application.Abstractions.Persistence;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Persistence;

namespace QuranSchool.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AttendanceRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AttendanceSession?> GetSessionByAllocationAndDateAsync(Guid allocationId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceSessions
            .FirstOrDefaultAsync(s => s.AllocationId == allocationId && s.SessionDate == date, cancellationToken);
    }

    public async Task AddSessionAsync(AttendanceSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.AttendanceSessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRecordAsync(AttendanceRecord record, CancellationToken cancellationToken = default)
    {
        await _dbContext.AttendanceRecords.AddAsync(record, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRecordAsync(AttendanceRecord record, CancellationToken cancellationToken = default)
    {
        _dbContext.AttendanceRecords.Update(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AttendanceRecord?> GetRecordBySessionAndStudentAsync(Guid sessionId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .FirstOrDefaultAsync(r => r.AttendanceSessionId == sessionId && r.StudentId == studentId, cancellationToken);
    }

    public async Task<List<AttendanceRecord>> GetRecordsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .Where(r => r.AttendanceSessionId == sessionId)
            .Include(r => r.Student)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AttendanceRecord>> GetRecordsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttendanceRecords
            .Where(r => r.StudentId == studentId)
            .Include(r => r.AttendanceSession)
            .ThenInclude(s => s!.Allocation)
            .ThenInclude(a => a!.Class)
            .Include(r => r.AttendanceSession)
            .ThenInclude(s => s!.Allocation)
            .ThenInclude(a => a!.Subject)
            .OrderByDescending(r => r.AttendanceSession!.SessionDate)
            .ToListAsync(cancellationToken);
    }
}
