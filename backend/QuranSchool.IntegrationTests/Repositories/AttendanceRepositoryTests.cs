using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class AttendanceRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddSessionAsync_ShouldSaveSession()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;

        await repo.AddSessionAsync(session);

        var result = await repo.GetSessionByIdAsync(session.Id);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddRecordAsync_ShouldSaveRecord()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);
        
        var student = await CreateStudent();
        var record = AttendanceRecord.Create(session.Id, student.Id, AttendanceStatus.Present).Value;

        await repo.AddRecordAsync(record);

        var result = await repo.GetRecordBySessionAndStudentAsync(session.Id, student.Id);
        result.Should().NotBeNull();
        result!.Status.Should().Be(AttendanceStatus.Present);
    }

    [Fact]
    public async Task GetRecordsByStudentIdAsync_ShouldReturnRecords()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);
        var student = await CreateStudent();
        var record = AttendanceRecord.Create(session.Id, student.Id, AttendanceStatus.Present).Value;
        await repo.AddRecordAsync(record);

        var result = await repo.GetRecordsByStudentIdAsync(student.Id);

        result.Should().Contain(r => r.Id == record.Id);
    }

    [Fact]
    public async Task UpdateRecordAsync_ShouldUpdateStatus()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);
        var student = await CreateStudent();
        var record = AttendanceRecord.Create(session.Id, student.Id, AttendanceStatus.Absent).Value;
        await repo.AddRecordAsync(record);

        record.Status = AttendanceStatus.Present;
        await repo.UpdateRecordAsync(record);

        var result = await repo.GetRecordBySessionAndStudentAsync(session.Id, student.Id);
        result!.Status.Should().Be(AttendanceStatus.Present);
    }

    [Fact]
    public async Task GetRecordsBySessionIdAsync_ShouldReturnRecords()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);
        var student = await CreateStudent();
        var record = AttendanceRecord.Create(session.Id, student.Id, AttendanceStatus.Present).Value;
        await repo.AddRecordAsync(record);

        var result = await repo.GetRecordsBySessionIdAsync(session.Id);
        result.Should().Contain(r => r.Id == record.Id);
    }

    [Fact]
    public async Task UpdateSessionAsync_ShouldUpdateDate()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);

        var newDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        session.Update(newDate);
        await repo.UpdateSessionAsync(session);

        var result = await repo.GetSessionByIdAsync(session.Id);
        result!.SessionDate.Should().Be(newDate);
    }

    [Fact]
    public async Task DeleteSessionAsync_ShouldRemoveSession()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var session = AttendanceSession.Create(allocation.Id, DateOnly.FromDateTime(DateTime.Now)).Value;
        await repo.AddSessionAsync(session);

        await repo.DeleteSessionAsync(session);

        var result = await repo.GetSessionByIdAsync(session.Id);
        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetSessionByAllocationAndDateAsync_ShouldReturnSession()
    {
        var repo = new AttendanceRepository(DbContext);
        var allocation = await CreateAllocation();
        var date = DateOnly.FromDateTime(DateTime.Now);
        var session = AttendanceSession.Create(allocation.Id, date).Value;
        await repo.AddSessionAsync(session);

        var result = await repo.GetSessionByAllocationAndDateAsync(allocation.Id, date);
        result.Should().NotBeNull();
        result!.Id.Should().Be(session.Id);
    }

    private async Task<Allocation> CreateAllocation()
    {
        var teacher = User.Create(Guid.NewGuid().ToString(), "h", "T", UserRole.Teacher).Value;
        var @class = Class.Create(Guid.NewGuid().ToString()).Value;
        var subject = Subject.Create(Guid.NewGuid().ToString()).Value;
        DbContext.Set<User>().Add(teacher);
        DbContext.Set<Class>().Add(@class);
        DbContext.Set<Subject>().Add(subject);
        var allocation = Allocation.Create(teacher.Id, @class.Id, subject.Id).Value;
        DbContext.Set<Allocation>().Add(allocation);
        await DbContext.SaveChangesAsync();
        return allocation;
    }

    private async Task<User> CreateStudent()
    {
        var student = User.Create(Guid.NewGuid().ToString(), "h", "S", UserRole.Student).Value;
        DbContext.Set<User>().Add(student);
        await DbContext.SaveChangesAsync();
        return student;
    }
}
