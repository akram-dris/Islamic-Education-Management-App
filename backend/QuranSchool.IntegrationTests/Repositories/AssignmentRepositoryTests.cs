using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class AssignmentRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveAssignmentToDatabase()
    {
        // Arrange
        var assignmentRepo = new AssignmentRepository(DbContext);
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        // Setup Teacher, Class, Subject, and Allocation
        var teacher = new User { Id = Guid.NewGuid(), Username = "t1", FullName = "T", Role = UserRole.Teacher, PasswordHash = "h" };
        await userRepo.AddAsync(teacher);
        var schoolClass = new Class { Id = Guid.NewGuid(), Name = "C1" };
        await classRepo.AddAsync(schoolClass);
        var subject = new Subject { Id = Guid.NewGuid(), Name = "S1" };
        await subjectRepo.AddAsync(subject);
        var allocation = new Allocation { Id = Guid.NewGuid(), TeacherId = teacher.Id, ClassId = schoolClass.Id, SubjectId = subject.Id };
        await allocationRepo.AddAsync(allocation);

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            AllocationId = allocation.Id,
            Title = "Test Assignment",
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        await assignmentRepo.AddAsync(assignment);

        // Assert
        var saved = await assignmentRepo.GetByIdAsync(assignment.Id);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be(assignment.Title);
    }
}
