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
    public async Task AddSessionAsync_ShouldSaveSessionToDatabase()
    {
        // Arrange
        var repository = new AttendanceRepository(DbContext);
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        // Setup
        var teacher = User.Create("t_att", "h", "T", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var cls = Class.Create("C_att").Value;
        await classRepo.AddAsync(cls);
        var sub = Subject.Create("S_att").Value;
        await subjectRepo.AddAsync(sub);
        var alc = Allocation.Create(teacher.Id, cls.Id, sub.Id).Value;
        await allocationRepo.AddAsync(alc);

        var session = AttendanceSession.Create(alc.Id, DateOnly.FromDateTime(DateTime.Now)).Value;

        // Act
        await repository.AddSessionAsync(session);

        // Assert
        var saved = await repository.GetSessionByAllocationAndDateAsync(alc.Id, session.SessionDate);
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(session.Id);
    }
}
