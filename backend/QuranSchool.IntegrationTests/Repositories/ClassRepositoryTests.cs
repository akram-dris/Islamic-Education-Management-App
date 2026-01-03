using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class ClassRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveClass()
    {
        var repo = new ClassRepository(DbContext);
        var @class = Class.Create("Class 1").Value;

        await repo.AddAsync(@class);

        var result = await repo.GetByIdAsync(@class.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Class 1");
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var repo = new ClassRepository(DbContext);
        var @class = Class.Create("Exists").Value;
        await repo.AddAsync(@class);

        var result = await repo.ExistsAsync("Exists");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        var repo = new ClassRepository(DbContext);
        var result = await repo.ExistsAsync("NotExists");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllClasses()
    {
        var repo = new ClassRepository(DbContext);
        await repo.AddAsync(Class.Create("C1").Value);
        await repo.AddAsync(Class.Create("C2").Value);

        var result = await repo.GetAllAsync();

        result.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateName()
    {
        var repo = new ClassRepository(DbContext);
        var @class = Class.Create("Old").Value;
        await repo.AddAsync(@class);

        @class.Update("New");
        await repo.UpdateAsync(@class);

        var result = await repo.GetByIdAsync(@class.Id);
        result!.Name.Should().Be("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveClass()
    {
        var repo = new ClassRepository(DbContext);
        var @class = Class.Create("Del").Value;
        await repo.AddAsync(@class);

        await repo.DeleteAsync(@class);

        var result = await repo.GetByIdAsync(@class.Id);
        result.Should().NotBeNull();
        result!.IsDeleted.Should().BeTrue();
    }
}