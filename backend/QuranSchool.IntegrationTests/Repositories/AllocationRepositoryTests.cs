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
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        var teacher = User.Create("teacher_add", "hash", "Teacher", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var schoolClass = Class.Create("Class Add").Value;
        await classRepo.AddAsync(schoolClass);
        var subject = Subject.Create("Subject Add").Value;
        await subjectRepo.AddAsync(subject);

        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject.Id).Value;

        await allocationRepo.AddAsync(allocation, default);

        var exists = await allocationRepo.ExistsAsync(teacher.Id, schoolClass.Id, subject.Id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAllocation()
    {
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        var teacher = User.Create("teacher_get", "hash", "Teacher", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var schoolClass = Class.Create("Class Get").Value;
        await classRepo.AddAsync(schoolClass);
        var subject = Subject.Create("Subject Get").Value;
        await subjectRepo.AddAsync(subject);

        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject.Id).Value;
        await allocationRepo.AddAsync(allocation);

        var result = await allocationRepo.GetByIdAsync(allocation.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(allocation.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAllocation()
    {
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        var teacher = User.Create("teacher_up", "hash", "Teacher", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var schoolClass = Class.Create("Class Up").Value;
        await classRepo.AddAsync(schoolClass);
        var subject1 = Subject.Create("Subject 1").Value;
        await subjectRepo.AddAsync(subject1);
        var subject2 = Subject.Create("Subject 2").Value;
        await subjectRepo.AddAsync(subject2);

        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject1.Id).Value;
        await allocationRepo.AddAsync(allocation);

        allocation.Update(teacher.Id, schoolClass.Id, subject2.Id);
        await allocationRepo.UpdateAsync(allocation);

        var result = await allocationRepo.GetByIdAsync(allocation.Id);
        result!.SubjectId.Should().Be(subject2.Id);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveAllocation()
    {
        var allocationRepo = new AllocationRepository(DbContext);
        var userRepo = new UserRepository(DbContext);
        var classRepo = new ClassRepository(DbContext);
        var subjectRepo = new SubjectRepository(DbContext);

        var teacher = User.Create("teacher_del", "hash", "Teacher", UserRole.Teacher).Value;
        await userRepo.AddAsync(teacher);
        var schoolClass = Class.Create("Class Del").Value;
        await classRepo.AddAsync(schoolClass);
        var subject = Subject.Create("Subject Del").Value;
        await subjectRepo.AddAsync(subject);

        var allocation = Allocation.Create(teacher.Id, schoolClass.Id, subject.Id).Value;
        await allocationRepo.AddAsync(allocation);

        await allocationRepo.DeleteAsync(allocation);

        var result = await allocationRepo.GetByIdAsync(allocation.Id);
        result.Should().BeNull();
    }
}