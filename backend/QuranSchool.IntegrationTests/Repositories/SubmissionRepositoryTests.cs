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
    public async Task AddAsync_ShouldSaveSubmission()
    {
        var repo = new SubmissionRepository(DbContext);
        var assignment = await CreateAssignment();
        var student = await CreateStudent();
        var submission = Submission.Create(assignment.Id, student.Id, "url").Value;

        await repo.AddAsync(submission);

        var result = await repo.GetByIdAsync(submission.Id);
        result.Should().NotBeNull();
        result!.FileUrl.Should().Be("url");
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var repo = new SubmissionRepository(DbContext);
        var assignment = await CreateAssignment();
        var student = await CreateStudent();
        var submission = Submission.Create(assignment.Id, student.Id, "url").Value;
        await repo.AddAsync(submission);

        var result = await repo.ExistsAsync(student.Id, assignment.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByAssignmentIdAsync_ShouldReturnSubmissions()
    {
        var repo = new SubmissionRepository(DbContext);
        var assignment = await CreateAssignment();
        var student = await CreateStudent();
        var submission = Submission.Create(assignment.Id, student.Id, "url").Value;
        await repo.AddAsync(submission);

        var result = await repo.GetByAssignmentIdAsync(assignment.Id);

        result.Should().Contain(s => s.Id == submission.Id);
    }

    private async Task<Assignment> CreateAssignment()
    {
        var teacher = User.Create(Guid.NewGuid().ToString(), "h", "T", UserRole.Teacher).Value;
        var @class = Class.Create(Guid.NewGuid().ToString()).Value;
        var subject = Subject.Create(Guid.NewGuid().ToString()).Value;
        DbContext.Set<User>().Add(teacher);
        DbContext.Set<Class>().Add(@class);
        DbContext.Set<Subject>().Add(subject);
        var allocation = Allocation.Create(teacher.Id, @class.Id, subject.Id).Value;
        DbContext.Set<Allocation>().Add(allocation);
        var assignment = Assignment.Create(allocation.Id, "T", "D", DateOnly.MaxValue).Value;
        DbContext.Set<Assignment>().Add(assignment);
        await DbContext.SaveChangesAsync();
        return assignment;
    }

    private async Task<User> CreateStudent()
    {
        var student = User.Create(Guid.NewGuid().ToString(), "h", "S", UserRole.Student).Value;
        DbContext.Set<User>().Add(student);
        await DbContext.SaveChangesAsync();
        return student;
    }
}
