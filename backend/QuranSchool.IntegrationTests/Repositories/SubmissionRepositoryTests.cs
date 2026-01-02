using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Domain.Enums;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class SubmissionRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveSubmissionToDatabase()
    {
        // Arrange
        var submissionRepo = new SubmissionRepository(DbContext);
        var assignmentRepo = new AssignmentRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var allocationRepo = new AllocationRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        // Setup
        var student = User.Create("s1", "h", "S", UserRole.Student).Value;
        await userRepo.AddAsync(student);
        var teacher = User.Create("t2", "h", "T", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var cls = Class.Create("C2").Value;
        await classRepo.AddAsync(cls);
        var sub = Subject.Create("S2").Value;
        await subjectRepo.AddAsync(sub);
        var alc = Allocation.Create(teacher.Id, cls.Id, sub.Id).Value;
        await allocationRepo.AddAsync(alc);
        var asg = Assignment.Create(alc.Id, "A", null, DateOnly.FromDateTime(DateTime.Now)).Value;
        await assignmentRepo.AddAsync(asg);

        var submisson = Submission.Create(asg.Id, student.Id, "url").Value;

        // Act
        await submissionRepo.AddAsync(submisson);

        // Assert
        var exists = await submissionRepo.ExistsAsync(student.Id, asg.Id);
        exists.Should().BeTrue();
    }
}
