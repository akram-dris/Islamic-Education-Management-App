using FluentAssertions;
using Xunit;
using QuranSchool.Domain.Entities;
using QuranSchool.Infrastructure.Repositories;
using QuranSchool.IntegrationTests.Abstractions;

namespace QuranSchool.IntegrationTests.Repositories;

public class ClassRepositoryTests : BaseIntegrationTest
{
    [Fact]
    public async Task AddAsync_ShouldSaveClassToDatabase()
    {
        // Arrange
        var repository = new ClassRepository(DbContext);
        var schoolClass = Class.Create("Integration Test Class").Value;

        // Act
        await repository.AddAsync(schoolClass, default);

        // Assert
        var exists = await repository.ExistsAsync(schoolClass.Name);
        exists.Should().BeTrue();
    }
}
