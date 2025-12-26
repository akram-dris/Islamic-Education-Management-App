using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class AllocationRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveAllocationToDatabase()
    {
        // Arrange
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        var teacher = new User
        {
            Id = Guid.NewGuid(),
            Username = "teacher_alloc",
            FullName = "Teacher Alloc",
            PasswordHash = "hash",
            Role = UserRole.Teacher,
            CreatedAt = DateTime.UtcNow
        };
        await userRepo.AddAsync(teacher);

        var schoolClass = new Class { Id = Guid.NewGuid(), Name = "Alloc Class" };
        await classRepo.AddAsync(schoolClass);

        var subject = new Subject { Id = Guid.NewGuid(), Name = "Alloc Subject" };
        await subjectRepo.AddAsync(subject);

        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TeacherId = teacher.Id,
            ClassId = schoolClass.Id,
            SubjectId = subject.Id
        };

        // Act
        await allocationRepo.AddAsync(allocation, default);

        // Assert
        var exists = await allocationRepo.ExistsAsync(teacher.Id, schoolClass.Id, subject.Id);
        exists.Should().BeTrue();
    }
}
