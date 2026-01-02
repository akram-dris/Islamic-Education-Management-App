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
    public async Task AddAsync_ShouldSaveAssignment()
    {
        var assignmentRepo = new AssignmentRepository(DbContext);
        var allocation = await CreateAllocation();
        var assignment = Assignment.Create(allocation.Id, "Title", "Desc", DateOnly.FromDateTime(DateTime.Now)).Value;

        await assignmentRepo.AddAsync(assignment);

        var result = await assignmentRepo.GetByIdAsync(assignment.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Title");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAssignment()
    {
        var assignmentRepo = new AssignmentRepository(DbContext);
        var allocation = await CreateAllocation();
        var assignment = Assignment.Create(allocation.Id, "Old", "Old", DateOnly.FromDateTime(DateTime.Now)).Value;
        await assignmentRepo.AddAsync(assignment);

        assignment.Update("New", "New", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
        await assignmentRepo.UpdateAsync(assignment);

        var result = await assignmentRepo.GetByIdAsync(assignment.Id);
        result!.Title.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAssignment()
    {
        var assignmentRepo = new AssignmentRepository(DbContext);
        var allocation = await CreateAllocation();
        var assignment = Assignment.Create(allocation.Id, "T", "D", DateOnly.FromDateTime(DateTime.Now)).Value;
        await assignmentRepo.AddAsync(assignment);

        await assignmentRepo.DeleteAsync(assignment);

        var result = await assignmentRepo.GetByIdAsync(assignment.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByClassIdAsync_ShouldReturnAssignments()
    {
        var assignmentRepo = new AssignmentRepository(DbContext);
        var allocation = await CreateAllocation();
        var assignment = Assignment.Create(allocation.Id, "T", "D", DateOnly.FromDateTime(DateTime.Now)).Value;
        await assignmentRepo.AddAsync(assignment);

        var result = await assignmentRepo.GetByClassIdAsync(allocation.ClassId);

        result.Should().Contain(a => a.Id == assignment.Id);
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
}