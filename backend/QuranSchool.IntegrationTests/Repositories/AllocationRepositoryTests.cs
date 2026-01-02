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

        var teacher = User.Create("teacher_alloc", "hash", "Teacher Alloc", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);

        var schoolClass = Class.Create("Alloc Class").Value;
        await classRepo.AddAsync(schoolClass);

        var subject = Subject.Create("Alloc Subject").Value;
        await subjectRepo.AddAsync(subject);

        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject.Id).Value;

        // Act
        await allocationRepo.AddAsync(allocation, default);

        // Assert
        var exists = await allocationRepo.ExistsAsync(teacher.Id, schoolClass.Id, subject.Id);
        exists.Should().BeTrue();
    }
}
