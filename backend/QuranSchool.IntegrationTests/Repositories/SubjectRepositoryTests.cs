using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class SubjectRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveSubject()
    {
        var repo = new SubjectRepository(DbContext);
        var subject = Subject.Create("Subject 1").Value;

        await repo.AddAsync(subject);

        var result = await repo.GetByIdAsync(subject.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Subject 1");
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var repo = new SubjectRepository(DbContext);
        var subject = Subject.Create("Exists").Value;
        await repo.AddAsync(subject);

        var result = await repo.ExistsAsync("Exists");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        var repo = new SubjectRepository(DbContext);
        var result = await repo.ExistsAsync("NotExists");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSubjects()
    {
        var repo = new SubjectRepository(DbContext);
        await repo.AddAsync(Subject.Create("S1").Value);
        await repo.AddAsync(Subject.Create("S2").Value);

        var result = await repo.GetAllAsync();

        result.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateName()
    {
        var repo = new SubjectRepository(DbContext);
        var subject = Subject.Create("Old").Value;
        await repo.AddAsync(subject);

        subject.Update("New");
        await repo.UpdateAsync(subject);

        var result = await repo.GetByIdAsync(subject.Id);
        result!.Name.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSubject()
    {
        var repo = new SubjectRepository(DbContext);
        var subject = Subject.Create("Del").Value;
        await repo.AddAsync(subject);

        await repo.DeleteAsync(subject);

        var result = await repo.GetByIdAsync(subject.Id);
        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }
}