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
        var teacher = new User { Id = Guid.NewGuid(), Username = "t_att", FullName = "T", Role = UserRole.Teacher, PasswordHash = "h" };
        await userRepo.AddAsync(teacher);
        var cls = new Class { Id = Guid.NewGuid(), Name = "C_att" };
        await classRepo.AddAsync(cls);
        var sub = new Subject { Id = Guid.NewGuid(), Name = "S_att" };
        await subjectRepo.AddAsync(sub);
        var alc = new Allocation { Id = Guid.NewGuid(), TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = sub.Id };
        await allocationRepo.AddAsync(alc);

        var session = new AttendanceSession
        {
            Id = Guid.NewGuid(),
            AllocationId = alc.Id,
            SessionDate = DateOnly.FromDateTime(DateTime.Now)
        };

        // Act
        await repository.AddSessionAsync(session);

        // Assert
        var saved = await repository.GetSessionByAllocationAndDateAsync(alc.Id, session.SessionDate);
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(session.Id);
    }
}
