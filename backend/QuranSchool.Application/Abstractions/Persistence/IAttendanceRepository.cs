using QuranSchool.Domain.Entities;

namespace QuranSchool.Application.Abstractions.Persistence;

public interface IAttendanceRepository
{
    Task<AttendanceSession?> GetSessionByAllocationAndDateAsync(Guid allocationId, DateOnly date, CancellationToken cancellationToken = default);
    Task AddSessionAsync(AttendanceSession session, CancellationToken cancellationToken = default);
    Task AddRecordAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    Task UpdateRecordAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> GetRecordBySessionAndStudentAsync(Guid sessionId, Guid studentId, CancellationToken cancellationToken = default);
    Task<List<AttendanceRecord>> GetRecordsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<List<AttendanceRecord>> GetRecordsByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<AttendanceSession?> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task UpdateSessionAsync(AttendanceSession session, CancellationToken cancellationToken = default);
    Task DeleteSessionAsync(AttendanceSession session, CancellationToken cancellationToken = default);
}
