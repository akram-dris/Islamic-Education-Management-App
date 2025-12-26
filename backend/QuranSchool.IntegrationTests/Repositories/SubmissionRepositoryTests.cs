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
        var student = new User { Id = Guid.NewGuid(), Username = "s1", FullName = "S", Role = UserRole.Student, PasswordHash = "h" };
        await userRepo.AddAsync(student);
        var teacher = new User { Id = Guid.NewGuid(), Username = "t2", FullName = "T", Role = UserRole.Teacher, PasswordHash = "h" };
        await userRepo.AddAsync(teacher);
        var cls = new Class { Id = Guid.NewGuid(), Name = "C2" };
        await classRepo.AddAsync(cls);
        var sub = new Subject { Id = Guid.NewGuid(), Name = "S2" };
        await subjectRepo.AddAsync(sub);
        var alc = new Allocation { Id = Guid.NewGuid(), TeacherId = teacher.Id, ClassId = cls.Id, SubjectId = sub.Id };
        await allocationRepo.AddAsync(alc);
        var asg = new Assignment { Id = Guid.NewGuid(), AllocationId = alc.Id, Title = "A", DueDate = DateOnly.FromDateTime(DateTime.Now) };
        await assignmentRepo.AddAsync(asg);

        var submisson = new Submission
        {
            Id = Guid.NewGuid(),
            AssignmentId = asg.Id,
            StudentId = student.Id,
            FileUrl = "url"
        };

        // Act
        await submissionRepo.AddAsync(submisson);

        // Assert
        var exists = await submissionRepo.ExistsAsync(student.Id, asg.Id);
        exists.Should().BeTrue();
    }
}
