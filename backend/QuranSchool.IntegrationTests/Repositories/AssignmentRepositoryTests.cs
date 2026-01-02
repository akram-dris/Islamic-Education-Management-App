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
        var teacher = User.Create("t1", "h", "T", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var schoolClass = Class.Create("C1").Value;
        await classRepo.AddAsync(schoolClass);
        var subject = Subject.Create("S1").Value;
        await subjectRepo.AddAsync(subject);
        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject.Id).Value;
        await allocationRepo.AddAsync(allocation);

        var assignment = Assignment.Create(
            allocation.Id,
            "Test Assignment",
            null,
            DateOnly.FromDateTime(DateTime.Now.AddDays(7))).Value;

        // Act
        await assignmentRepo.AddAsync(assignment);

        // Assert
        var saved = await assignmentRepo.GetByIdAsync(assignment.Id);
        saved.Should().NotBeNull();
        saved!.Title.Should().Be(assignment.Title);
    }
}
